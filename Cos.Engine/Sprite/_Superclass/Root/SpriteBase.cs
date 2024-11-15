using Cos.Library.Mathematics;
using Cos.Library.Sprite;
using System.Drawing;

namespace Cos.Engine.Sprite._Superclass._Root
{
    /// <summary>
    /// Represents a single item that can be rendered to the screen. All on-screen objects are derived from this class.
    /// </summary>
    public partial class SpriteBase : ISprite
    {
        protected EngineCore _engine;

        private SharpDX.Direct2D1.Bitmap? _image;
        private bool _readyForDeletion;
        private CosVector _location = new();
        private Size _size;

        public SpriteBase(EngineCore engine, string spriteTag = "")
        {
            _engine = engine;

            SpriteTag = spriteTag;
            Orientation = new CosVector();
        }

        public void QueueForDelete()
        {
            _readyForDeletion = true;
            Visible = false;

            OnQueuedForDelete?.Invoke(this);
        }

        public void Reset()
        {
            _readyForDeletion = false;
            Visible = true;
        }

        /// <summary>
        /// Sets the sprites center to the center of the screen.
        /// </summary>
        public void CenterInUniverse()
        {
            X = _engine.Display.CanvasSize.Width / 2 /*- Size.Width / 2*/;
            Y = _engine.Display.CanvasSize.Height / 2 /*- Size.Height / 2*/;
        }

        public void SetImage(SharpDX.Direct2D1.Bitmap bitmap)
        {
            _image = bitmap;
            _size = new Size((int)_image.Size.Width, (int)_image.Size.Height);
        }

        public void SetImage(string imagePath)
        {
            _image = _engine.Assets.GetBitmap(imagePath);
            _size = new Size((int)_image.Size.Width, (int)_image.Size.Height);
        }

        /// <summary>
        /// Sets the size of the sprite. This is generally set by a call to SetImage() but some sprites (such as particles) have no images.
        /// </summary>
        /// <param name="size"></param>
        public void SetSize(Size size)
            => _size = size;

        /// <summary>
        /// Moves the sprite based on its movement vector and the epoch.
        /// </summary>
        /// <param name="displacementVector"></param>
        public virtual void ApplyMotion(float epoch, CosVector displacementVector)
        {
            //Perform any auto-rotation.
            Orientation.Radians += RotationSpeed * epoch;

            //Move the sprite based on its vector.
            Location += MovementVector * epoch;
        }

        public virtual void Cleanup()
        {
            Visible = false;
        }
    }
}
