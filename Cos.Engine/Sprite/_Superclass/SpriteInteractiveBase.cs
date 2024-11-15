using Cos.Engine.Sprite._Superclass._Root;
using Cos.GameEngine.Sprite.SupportingClasses.Metadata;
using Cos.Library;
using Cos.Library.Mathematics;
using SharpDX.Direct2D1;
using System;

namespace Cos.Engine.Sprite._Superclass
{
    /// <summary>
    /// A sprite that the player can see, probably shoot and destroy and might even shoot back.
    /// </summary>
    public class SpriteInteractiveBase : SpriteBase
    {
        public CosRenewableResources RenewableResources { get; set; } = new();
        private InteractiveSpriteMetadata? _metadata = null;
        public InteractiveSpriteMetadata Metadata => _metadata ?? throw new NullReferenceException();

        public SpriteInteractiveBase(EngineCore engine, string? imagePath)
            : base(engine)
        {
            _engine = engine;

            if (imagePath != null)
            {
                SetImageAndLoadMetadata(imagePath);
            }
        }

        public SpriteInteractiveBase(EngineCore engine, Bitmap bitmap)
            : base(engine)
        {
            _engine = engine;

            SetImage(bitmap);
        }

        /// <summary>
        /// Sets the sprites image, sets speed, shields, adds attachments and weapons
        /// from a .json file in the same path with the same name as the sprite image.
        /// </summary>
        /// <param name="spriteImagePath"></param>
        private void SetImageAndLoadMetadata(string spriteImagePath)
        {
            //_metadata = _engine.Assets.GetMetaData<InteractiveSpriteMetadata>(spriteImagePath);
            _metadata = new();

            SetImage(spriteImagePath);

            // Set standard variables here:
            Speed = Metadata.Speed;
        }

        public override void Explode()
        {
            _engine.Events.Add(() =>
            {
                _engine.Sprites.Animations.AddRandomExplosionAt(this);
                _engine.Sprites.Particles.ParticleBlastAt(CosRandom.Between(200, 800), this);
                _engine.Sprites.CreateFragmentsOf(this);
                _engine.Rendering.AddScreenShake(4, 800);
                _engine.Audio.PlayRandomExplosion();
            });

            base.Explode();
        }

        /// <summary>
        /// Provides a way to make decisions about the sprite that do not necessarily have anything to do with movement.
        /// </summary>
        /// <param name="epoch"></param>
        /// <param name="displacementVector"></param>
        public virtual void ApplyIntelligence(float epoch, CosVector displacementVector)
        {
        }
    }
}