using Cos.Engine.Sprite.Enemy.Peon._Superclass;
using Cos.Library.ExtensionMethods;
using Cos.Library.Mathematics;
using System.Drawing;
using static Cos.Library.CosConstants;

namespace Cos.Engine.Sprite.Enemy.Debug
{
    /// <summary>
    /// Debugging enemy unit - a scary sight to see.
    /// </summary>
    internal class SpriteEnemyDebug : SpriteEnemyPeonBase
    {
        public SpriteEnemyDebug(EngineCore engine)
            : base(engine, @"Sprites\Enemy\Debug\Hull.png")
        {
            _particle1 = _engine.Sprites.Particles.AddAt(CosVector.Zero, new Size(5, 5));
            _particle1.Pattern = CosParticleColorType.Solid;
            _particle1.Color = _engine.Rendering.Materials.Colors.Red;
            _particle1.Shape = CosParticleShape.HollowEllipse;
            _particle1.RotationSpeed = 0;
            _particle1.RecalculateMovementVector();

            _particle2 = _engine.Sprites.Particles.AddAt(CosVector.Zero, new Size(5, 5));
            _particle2.Pattern = CosParticleColorType.Solid;
            _particle2.Color = _engine.Rendering.Materials.Colors.Green;
            _particle2.Shape = CosParticleShape.FilledEllipse;
            _particle2.RotationSpeed = 0;
            _particle2.RecalculateMovementVector();

            _particle3 = _engine.Sprites.Particles.AddAt(CosVector.Zero, new Size(10, 10));
            _particle3.Pattern = CosParticleColorType.Solid;
            _particle3.Color = _engine.Rendering.Materials.Colors.Blue;
            _particle3.Shape = CosParticleShape.HollowRectangle;
            _particle3.RotationSpeed = 0.02f;
            _particle3.RecalculateMovementVector();

            _particle4 = _engine.Sprites.Particles.AddAt(CosVector.Zero, new Size(10, 10));
            _particle4.Pattern = CosParticleColorType.Solid;
            _particle4.Color = _engine.Rendering.Materials.Colors.Cyan;
            _particle4.Shape = CosParticleShape.Triangle;
            _particle4.RotationSpeed = 0.02f;
            _particle4.RecalculateMovementVector();
        }

        SpriteParticle _particle1;
        SpriteParticle _particle2;
        SpriteParticle _particle3;
        SpriteParticle _particle4;

        public override void ApplyIntelligence(float epoch, CosVector displacementVector)
        {
            Orientation.RadiansSigned += 0.05f; // = this.AngleToInSignedRadians(_engine.Player.Sprite);

            var point1 = Orientation.RotatedBy(90.ToRadians()) * new CosVector(50, 50);
            _particle1.Location = Location + point1;

            var point2 = Orientation.RotatedBy(-90.ToRadians()) * new CosVector(50, 50);
            _particle2.Location = Location + point2;

            var point3 = Orientation * new CosVector(50, 50);
            _particle3.Location = Location + point3;

            var point4 = Orientation * new CosVector(50, 50) * -1;
            _particle4.Location = Location + point4;

            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}

