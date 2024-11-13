using Cos.Library;
using Cos.Library.ExtensionMethods;
using Cos.Library.Mathematics;
using Cos.Library.Sprite;
using SharpDX.Mathematics.Interop;
using System.Drawing;

namespace Cos.Engine.Sprite._Superclass._Root
{
    /// <summary>
    /// Represents a single item that can be rendered to the screen. All on-screen objects are derived from this class.
    /// </summary>
    public partial class SpriteBase : ISprite
    {
        #region Travel Vector.

        private float _speed;
        /// <summary>
        /// The speed that this object can generally travel in any direction.
        /// </summary>
        public float Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                //RecalculateMovementVector(); //Seems like unneeded overhead.
            }
        }

        /// <summary>
        /// Omni-directional velocity.
        /// </summary>
        public CosVector MovementVector { get; set; } = new();

        private float _throttle = 1.0f;
        /// <summary>
        /// Percentage of speed expressed as a decimal percentage from 0.0 (stopped) to float.max.
        /// Note that a throttle of 2.0 is twice the normal speed.
        /// </summary>
        public float Throttle
        {
            get => _throttle;
            set
            {
                _throttle = value.Clamp(0, float.MaxValue);
                //RecalculateMovementVector(); //Seems like unneeded overhead.
            }
        }

        private float _maxThrottle = 1.0f;
        /// <summary>
        /// The general maximum throttle that can be applied. This can be considered the "boost" speed.
        /// </summary>
        public float MaxThrottle
        {
            get => _maxThrottle;
            set => _maxThrottle = value.Clamp(0, float.MaxValue);
        }

        #endregion


        /// <summary>
        /// Number or radians to rotate the sprite Orientation along its center at each call to ApplyMotion().
        /// Negative for counter-clockwise, positive for clockwise.
        /// </summary>
        public float RotationSpeed { get; set; } = 0;

        private CosVector _orientation = new CosVector();
        /// <summary>
        /// The angle in which the sprite is pointing, note that this is NOT the travel angle.
        /// The travel angle is baked into the MovementVector. If you need the movement vector
        /// to follow this direction angle then call RecalculateMovementVector() after modifying
        /// the PointingAngle.
        /// </summary>
        public CosVector Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                _orientation.OnChangeEvent += (CosVector vector) => RotationChanged();
                RotationChanged();
            }
        }

        public SharpDX.Direct2D1.Bitmap? GetImage() => _image;
        public string SpriteTag { get; set; }
        public uint UID { get; private set; } = CosSequenceGenerator.Next();
        public uint OwnerUID { get; set; }
        public bool IsWithinCurrentScaledScreenBounds => _engine.Display.GetCurrentScaledScreenBounds().IntersectsWith(RenderBounds);
        public int HullHealth { get; private set; } = 0; //Ship hit-points.
        public int ShieldHealth { get; private set; } = 0; //Shield hit-points, these take 1/2 damage.

        /// <summary>
        /// The sprite still exists, but is not functional (e.g. its been shot and exploded).
        /// </summary>
        public bool IsDeadOrExploded { get; private set; } = false;
        public bool IsQueuedForDeletion => _readyForDeletion;

        /// <summary>
        /// If true, the sprite does not respond to changes in background offset.
        /// </summary>
        public bool IsFixedPosition { get; set; }

        /// <summary>
        /// Width and height of the sprite.
        /// </summary>
        public virtual Size Size => _size;

        /// <summary>
        /// The bounds of the sprite in the universe.
        /// </summary>
        public virtual RectangleF Bounds => new(
                Location.X - Size.Width / 2.0f,
                Location.Y - Size.Height / 2.0f,
                Size.Width,
                Size.Height);

        /// <summary>
        /// The raw bounds of the sprite in the universe.
        /// </summary>
        public virtual RawRectangleF RawBounds => new(
                        Location.X - Size.Width / 2.0f,
                        Location.Y - Size.Height / 2.0f,
                        Location.X - Size.Width / 2.0f + Size.Width,
                        Location.Y - Size.Height / 2.0f + Size.Height);

        /// <summary>
        /// The bounds of the sprite on the display.
        /// </summary>
        public virtual RectangleF RenderBounds => new(
                        RenderLocation.X - Size.Width / 2.0f,
                        RenderLocation.Y - Size.Height / 2.0f,
                        Size.Width,
                        Size.Height);

        /// <summary>
        /// The raw bounds of the sprite on the display.
        /// </summary>
        public virtual RawRectangleF RawRenderBounds => new(
                        RenderLocation.X - Size.Width / 2.0f,
                        RenderLocation.Y - Size.Height / 2.0f,
                        RenderLocation.X - Size.Width / 2.0f + Size.Width,
                        RenderLocation.Y - Size.Height / 2.0f + Size.Height);


        /// <summary>
        /// The x,y, location of the center of the sprite in the universe.
        /// Do not modify the X,Y of the returned location, it will have no effect.
        /// </summary>
        public CosVector Location
        {
            get => _location.Clone(); //Changes made to the location object do not affect the sprite.
            set
            {
                _location = value;
                LocationChanged();
            }
        }

        /// <summary>
        /// The top left corner of the sprite in the universe.
        /// </summary>
        public CosVector LocationTopLeft
        {
            get => _location - Size / 2.0f; //Changes made to the location object do not affect the sprite.
            set
            {
                _location = value;
                LocationChanged();
            }
        }

        /// <summary>
        /// The x,y, location of the center of the sprite on the screen.
        /// Do not modify the X,Y of the returned location, it will have no effect.
        /// </summary>
        public CosVector RenderLocation
        {
            get
            {
                if (IsFixedPosition)
                {
                    return _location;
                }
                else
                {
                    return _location - _engine.Display.RenderWindowPosition;
                }
            }
        }

        /// <summary>
        /// The X location of the center of the sprite in the universe.
        /// </summary>
        public float X
        {
            get => _location.X;
            set
            {
                _location.X = value;
                LocationChanged();
            }
        }

        /// <summary>
        /// The Y location of the center of the sprite in the universe.
        /// </summary>
        public float Y
        {
            get => _location.Y;
            set
            {
                _location.Y = value;
                LocationChanged();
            }
        }

        // The Z location. Given that this is a 2d engine, the Z order is just a render order.
        public int Z { get; set; } = 0;

        private bool _isVisible = true;
        public bool Visible
        {
            get => _isVisible && !_readyForDeletion;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnVisibilityChanged?.Invoke(this);
                    VisibilityChanged();
                }
            }
        }
    }
}
