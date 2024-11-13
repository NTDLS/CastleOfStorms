using Cos.Engine;
using Cos.Engine.Manager;
using Cos.Engine.Sprite;
using Cos.Engine.Sprite._Superclass;
using Cos.Engine.Sprite._Superclass._Root;
using Cos.Engine.TickController._Superclass;
using Cos.Library;
using Cos.Library.Mathematics;
using Cos.Rendering;
using SharpDX;
using System.Drawing;
using static Cos.Library.CosConstants;

namespace Cos.GameEngine.TickController.VectoredTickController.Uncollidable
{
    public class ParticleSpriteTickController : VectoredTickControllerBase<SpriteParticleBase>
    {
        public ParticleSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, CosVector displacementVector)
        {
            foreach (var particle in Visible())
            {
                particle.ApplyMotion(epoch, displacementVector);
            }
        }

        public void AddAt(CosVector location, Color4 color, int count, Size? size = null)
        {
            for (int i = 0; i < count; i++)
            {
                AddAt(location + CosRandom.Between(-20, 20), color, size);
            }
        }

        public void AddAt(SpriteBase sprite, Color4 color, int count, Size? size = null)
        {
            for (int i = 0; i < count; i++)
            {
                AddAt(sprite.Location + CosRandom.Between(-20, 20), color, size);
            }
        }

        public SpriteParticle AddAt(SpriteBase sprite, Color4 color, Size? size = null)
        {
            var obj = new SpriteParticle(Engine, sprite.Location, size ?? new Size(1, 1), color);
            SpriteManager.Add(obj);
            return obj;
        }

        public SpriteParticle AddAt(CosVector location, Color4 color, Size? size = null)
        {
            var obj = new SpriteParticle(Engine, location, size ?? new Size(1, 1), color)
            {
                Visible = true
            };
            SpriteManager.Add(obj);
            return obj;
        }

        public SpriteParticle AddAt(CosVector location, Size? size = null)
        {
            var obj = new SpriteParticle(Engine, location, size ?? new Size(1, 1))
            {
                Visible = true
            };
            SpriteManager.Add(obj);
            return obj;
        }

        public void ParticleBlastAt(int maxParticleCount, SpriteBase at)
        {
            Engine.Events.Add(() => ParticleBlastAt(maxParticleCount, at.Location));
        }

        /// <summary>
        /// Creates a random number of blasts consisting of "hot" colored particles at a given location.
        /// </summary>
        /// <param name="maxParticleCount"></param>
        /// <param name="at"></param>
        public void ParticleBlastAt(int maxParticleCount, CosVector location)
        {
            for (int i = 0; i < CosRandom.Between(maxParticleCount / 2, maxParticleCount); i++)
            {
                var particle = AddAt(location, new Size(CosRandom.Between(1, 2), CosRandom.Between(1, 2)));
                particle.Shape = CosParticleShape.FilledEllipse;
                particle.Pattern = CosParticleColorType.Solid;
                //particle.GradientStartColor = CosRenderingUtility.GetRandomHotColor();
                //particle.GradientEndColor = CosRenderingUtility.GetRandomHotColor();
                particle.Color = CosRenderingUtility.GetRandomHotColor();
                particle.CleanupMode = CosParticleCleanupMode.FadeToBlack;
                particle.FadeToBlackReductionAmount = CosRandom.Between(0.001f, 0.01f);
                particle.Speed *= CosRandom.Between(1, 3.5f);
                particle.VectorType = CosParticleVectorType.Default;
            }
        }

        public void ParticleCloud(int particleCount, SpriteBase at)
            => ParticleCloud(particleCount, at.Location);

        public void ParticleCloud(int particleCount, CosVector location)
        {
            for (int i = 0; i < particleCount; i++)
            {
                var particle = AddAt(location, CosRenderingUtility.GetRandomHotColor(), new Size(5, 5));

                switch (CosRandom.Between(1, 3))
                {
                    case 1:
                        particle.Shape = CosParticleShape.Triangle;
                        break;
                    case 2:
                        particle.Shape = CosParticleShape.FilledEllipse;
                        break;
                    case 3:
                        particle.Shape = CosParticleShape.HollowEllipse;
                        break;
                }

                particle.CleanupMode = CosParticleCleanupMode.FadeToBlack;
                particle.FadeToBlackReductionAmount = 0.001f;
                particle.RotationSpeed = CosRandom.Between(-3f, 3f);
                particle.VectorType = CosParticleVectorType.FollowOrientation;
                particle.Orientation.Degrees = CosRandom.Between(0.0f, 359.0f);
                particle.Speed = CosRandom.Between(2, 3.5f);
                particle.RecalculateMovementVector();
            }
        }
    }
}
