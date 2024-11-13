using Cos.Library.ExtensionMethods;
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
            IsHighlighted = _engine.Settings.HighlightAllSprites;
            Orientation = new CosVector();
        }

        public void QueueForDelete()
        {
            _readyForDeletion = true;
            Visible = false;

            OnQueuedForDelete?.Invoke(this);
        }

        /// <summary>
        /// Sets the sprites center to the center of the screen.
        /// </summary>
        public void CenterInUniverse()
        {
            X = _engine.Display.TotalCanvasSize.Width / 2 /*- Size.Width / 2*/;
            Y = _engine.Display.TotalCanvasSize.Height / 2 /*- Size.Height / 2*/;
        }

        public void SetHullHealth(int points)
        {
            HullHealth = 0;
            AddHullHealth(points);
        }

        public virtual void AddHullHealth(int pointsToAdd)
            => HullHealth = (HullHealth + pointsToAdd).Clamp(1, _engine.Settings.MaxHullHealth);

        public virtual void SetShieldHealth(int points)
        {
            ShieldHealth = 0;
            AddShieldHealth(points);
        }

        public virtual void AddShieldHealth(int pointsToAdd)
            => ShieldHealth = (ShieldHealth + pointsToAdd).Clamp(1, _engine.Settings.MaxShieldHealth);

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

            _engine.Sprites.QueueAllForDeletionByOwner(UID);
        }

        public string GetInspectionText()
        {
            string extraInfo = string.Empty;

            return
                  $">                    UID: {UID}\r\n"
                + $"               Owner UID: {OwnerUID:n0}\r\n"
                + $"                    Type: {GetType().Name}\r\n"
                + $"                     Tag: {SpriteTag:n0}\r\n"
                + $"             Is Visible?: {Visible:n0}\r\n"
                + $"                    Size: {Size:n0}\r\n"
                + $"                  Bounds: {Bounds:n0}\r\n"
                + $"       Ready for Delete?: {IsQueuedForDeletion}\r\n"
                + $"                Is Dead?: {IsDeadOrExploded}\r\n"
                + $"         Render-Location: {RenderLocation}\r\n"
                + $"                Location: {Location}\r\n"
                + $"                   Angle: {Orientation}\r\n"
                + $"                          {Orientation.DegreesSigned:n2}deg\r\n"
                + $"                          {Orientation.RadiansSigned:n2}rad\r\n"
                + extraInfo
                + $"       Background Offset: {_engine.Display.RenderWindowPosition}\r\n"
                + $"                  Thrust: {MovementVector * 100:n2}\r\n"
                + $"                   Boost: {Throttle * 100:n2}\r\n"
                + $"                    Hull: {HullHealth:n0}\r\n"
                + $"                  Shield: {ShieldHealth:n0}\r\n"
                + $"               Highlight: {IsHighlighted}\r\n"
                + $"       Is Fixed Position: {IsFixedPosition}\r\n"
                //+ $"            Is Locked On: {IsLockedOnHard}\r\n"
                //+ $"     Is Locked On (Soft): {IsLockedOnSoft:n0}\r\n"
                + $"In Current Scaled Bounds: {IsWithinCurrentScaledScreenBounds}\r\n"
                + $"          Visible Bounds: {Bounds}\r\n";
        }

    }
}
