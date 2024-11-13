using Cos.Engine.Sprite._Superclass;
using Cos.Library.ExtensionMethods;
using Cos.Library.Mathematics;
using System;
using static Cos.Library.CosConstants;

namespace Cos.Engine.Sprite
{
    /// <summary>
    /// These are generic collidable, interactive bitmap sprites. They can take damage and even shoot back.
    /// </summary>
    public class SpriteInteractiveBitmap : SpriteInteractiveBase
    {
        /// <summary>
        /// The max travel distance from the creation x,y before the sprite is automatically deleted.
        /// This is ignored unless the CleanupModeOption is Distance.
        /// </summary>
        public float MaxDistance { get; set; } = 1000;

        /// <summary>
        /// The amount of brightness to reduce the color by each time the particle is rendered.
        /// This is ignored unless the CleanupModeOption is FadeToBlack.
        /// This should be expressed as a number between 0-1 with 0 being no reduction per frame and 1 being 100% reduction per frame.
        /// </summary>
        public float FadeToBlackReductionAmount { get; set; } = 0.01f;

        public CosParticleVectorType VectorType { get; set; } = CosParticleVectorType.Default;

        public CosParticleCleanupMode CleanupMode { get; set; } = CosParticleCleanupMode.None;

        public SpriteInteractiveBitmap(EngineCore engine, string imagePath)
            : base(engine, imagePath)
        {
        }

        public SpriteInteractiveBitmap(EngineCore engine, SharpDX.Direct2D1.Bitmap bitmap)
            : base(engine, bitmap)
        {
        }

        public override void ApplyMotion(float epoch, CosVector displacementVector)
        {
            Orientation.Degrees += RotationSpeed * epoch;

            if (VectorType == CosParticleVectorType.FollowOrientation)
            {
                RecalculateMovementVector(Orientation.RadiansSigned);
            }

            base.ApplyMotion(epoch, displacementVector);

            if (CleanupMode == CosParticleCleanupMode.FadeToBlack)
            {
                throw new NotImplementedException();
                /*
                Color *= 1 - (float)FadeToBlackReductionAmount; // Gradually darken the particle color.

                // Check if the particle color is below a certain threshold and remove it.
                if (Color.Red < 0.5f && Color.Green < 0.5f && Color.Blue < 0.5f)
                {
                    QueueForDelete();
                }
                */
            }
            else if (CleanupMode == CosParticleCleanupMode.DistanceOffScreen)
            {
                if (_engine.Display.TotalCanvasBounds.Balloon(MaxDistance).IntersectsWith(RenderBounds) == false)
                {
                    QueueForDelete();
                }
            }
        }
    }
}
