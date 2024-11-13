using Cos.Engine.Sprite._Superclass;
using Cos.Library;
using Cos.Library.ExtensionMethods;
using Cos.Library.Mathematics;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System.Drawing;
using static Cos.Library.CosConstants;

namespace Cos.Engine.Sprite
{
    public class SpriteParticle : SpriteParticleBase
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
        public CosParticleColorType Pattern { get; set; } = CosParticleColorType.Solid;
        public CosParticleVectorType VectorType { get; set; } = CosParticleVectorType.Default;
        public CosParticleShape Shape { get; set; } = CosParticleShape.FilledEllipse;
        public CosParticleCleanupMode CleanupMode { get; set; } = CosParticleCleanupMode.None;

        /// <summary>
        /// The color of the particle when ColorType == Color;
        /// </summary>
        public Color4 Color { get; set; }

        /// <summary>
        /// The color of the particle when ColorType == Gradient;
        /// </summary>
        public Color4 GradientStartColor { get; set; }
        /// <summary>
        /// The color of the particle when ColorType == Gradient;
        /// </summary>
        public Color4 GradientEndColor { get; set; }

        public SpriteParticle(EngineCore engine, CosVector location, Size size, Color4? color = null)
            : base(engine)
        {
            SetSize(size);

            Location = location.Clone();

            Color = color ?? engine.Rendering.Materials.Colors.White;
            RotationSpeed = CosRandom.Between(0.01f, 0.09f) * CosRandom.PositiveOrNegative();

            Speed = CosRandom.Between(1.0f, 4.0f);
            Orientation.Degrees = CosRandom.Between(0, 359);
            Throttle = 1;

            RecalculateMovementVector();

            _engine = engine;
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
                if (Pattern == CosParticleColorType.Solid)
                {
                    Color *= 1 - (float)FadeToBlackReductionAmount; // Gradually darken the particle color.

                    // Check if the particle color is below a certain threshold and remove it.
                    if (Color.Red < 0.5f && Color.Green < 0.5f && Color.Blue < 0.5f)
                    {
                        QueueForDelete();
                    }
                }
                else if (Pattern == CosParticleColorType.Gradient)
                {
                    GradientStartColor *= 1 - (float)FadeToBlackReductionAmount; // Gradually darken the particle color.
                    GradientEndColor *= 1 - (float)FadeToBlackReductionAmount; // Gradually darken the particle color.

                    // Check if the particle color is below a certain threshold and remove it.
                    if (GradientStartColor.Red < 0.5f && GradientStartColor.Green < 0.5f && GradientStartColor.Blue < 0.5f
                        || GradientEndColor.Red < 0.5f && GradientEndColor.Green < 0.5f && GradientEndColor.Blue < 0.5f)
                    {
                        QueueForDelete();
                    }
                }
            }
            else if (CleanupMode == CosParticleCleanupMode.DistanceOffScreen)
            {
                if (_engine.Display.TotalCanvasBounds.Balloon(MaxDistance).IntersectsWith(RenderBounds) == false)
                {
                    QueueForDelete();
                }
            }
        }

        public override void Render(RenderTarget renderTarget)
        {
            if (Visible)
            {
                switch (Shape)
                {
                    case CosParticleShape.FilledEllipse:
                        if (Pattern == CosParticleColorType.Solid)
                        {
                            _engine.Rendering.DrawSolidEllipse(renderTarget,
                                RenderLocation.X, RenderLocation.Y, Size.Width, Size.Height, Color, (float)Orientation.Degrees);
                        }
                        else if (Pattern == CosParticleColorType.Gradient)
                        {
                            _engine.Rendering.DrawGradientEllipse(renderTarget, RenderLocation.X, RenderLocation.Y,
                                Size.Width, Size.Height, GradientStartColor, GradientEndColor, (float)Orientation.Degrees);
                        }
                        break;
                    case CosParticleShape.HollowEllipse:
                        _engine.Rendering.DrawEllipse(renderTarget,
                            RenderLocation.X, RenderLocation.Y, Size.Width, Size.Height, Color, 1, (float)Orientation.Degrees);
                        break;

                    case CosParticleShape.FilledRectangle:
                        {
                            var rect = new RawRectangleF(0, 0, Size.Width, Size.Height);

                            if (Pattern == CosParticleColorType.Solid)
                            {
                                _engine.Rendering.DrawSolidRectangle(renderTarget, RenderLocation.X - Size.Width / 2,
                                    RenderLocation.Y - Size.Height / 2, rect, Color, 0, (float)Orientation.Degrees);
                            }
                            else if (Pattern == CosParticleColorType.Gradient)
                            {
                                _engine.Rendering.DrawGradientRectangle(renderTarget, RenderLocation.X - Size.Width / 2,
                                    RenderLocation.Y - Size.Height / 2, rect, GradientStartColor, GradientEndColor, 0, (float)Orientation.Degrees);
                            }
                        }
                        break;

                    case CosParticleShape.HollowRectangle:
                        {
                            var rect = new RawRectangleF(0, 0, Size.Width, Size.Height);
                            _engine.Rendering.DrawRectangle(renderTarget, RenderLocation.X - Size.Width / 2,
                                RenderLocation.Y - Size.Height / 2, rect, Color, 0, 1, (float)Orientation.Degrees);

                        }
                        break;

                    case CosParticleShape.Triangle:
                        _engine.Rendering.DrawTriangle(renderTarget,
                            RenderLocation.X, RenderLocation.Y, Size.Width, Size.Height, Color, 1, (float)Orientation.Degrees);
                        break;
                }

                if (IsHighlighted)
                {
                    _engine.Rendering.DrawRectangle(renderTarget, RawRenderBounds,
                        _engine.Rendering.Materials.Colors.Red, 0, 1, Orientation.RadiansSigned);
                }
            }
        }
    }
}
