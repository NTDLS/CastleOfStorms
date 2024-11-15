using Cos.Engine.Sprite._Superclass;
using Cos.Library.Mathematics;

namespace Cos.Engine.Sprite
{
    public class SpriteDebug : SpriteInteractiveShipBase
    {
        public SpriteDebug(EngineCore engine)
            : base(engine, @"Sprites\Debug.png")
        {
            Initialize();
        }

        public SpriteDebug(EngineCore engine, float x, float y)
            : base(engine, @"Sprites\Debug.png")
        {
            Initialize();
            X = x;
            Y = y;
        }

        public SpriteDebug(EngineCore engine, float x, float y, string imagePath)
            : base(engine, imagePath)
        {
            Initialize();
            X = x;
            Y = y;
        }

        private void Initialize()
        {
            Speed = 1.5f;
            RecalculateMovementVector();
        }

        public override void ApplyMotion(float epoch, CosVector displacementVector)
        {
            Orientation.Degrees += 0.1f;

            //var deltaAngleS = this.HeadingAngleToInSignedDegrees(_engine.Player.Sprite.Location);
            //var deltaAngleU = this.HeadingAngleToInUnsignedDegrees(_engine.Player.Sprite);
            //System.Diagnostics.Debug.WriteLine($"U {deltaAngleU:n2}    S {deltaAngleS:n2}");

            base.ApplyMotion(epoch, displacementVector);
        }
    }
}
