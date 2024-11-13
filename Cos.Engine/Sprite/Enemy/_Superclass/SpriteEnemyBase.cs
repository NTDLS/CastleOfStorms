using Cos.Engine.Sprite._Superclass;
using Cos.Library;
using Cos.Library.Mathematics;

namespace Cos.Engine.Sprite.Enemy._Superclass
{
    /// <summary>
    /// The enemy base is a sub-class of the ship base. It is used by Peon and Boss enemies.
    /// </summary>
    public class SpriteEnemyBase : SpriteInteractiveShipBase
    {
        public SpriteEnemyBase(EngineCore engine, string imagePath)
                : base(engine, imagePath)
        {
            RecalculateMovementVector();
        }

        public virtual void BeforeCreate() { }

        public virtual void AfterCreate() { }

        public override void RotationChanged() => LocationChanged();

        #region Artificial Intelligence.


        #endregion

        public override void Explode()
        {
            _engine.Player.Stats.Bounty += Metadata.Bounty;

            if (CosRandom.PercentChance(10))
            {
            }
            base.Explode();
        }

        /// <summary>
        /// Moves the sprite based on its velocity/boost (velocity) taking into account the background scroll.
        /// </summary>
        /// <param name="displacementVector"></param>
        public override void ApplyMotion(float epoch, CosVector displacementVector)
        {
            /*
            //When an enemy has boost available, it will use it.
            if (AvailableBoost > 0)
            {
                if (ThrottlePercentage < 1.0) //Ramp up the boost until it is at 100%
                {
                    ThrottlePercentage += _engine.Settings.EnemyVelocityRampUp;
                }
                AvailableBoost -= MaximumBoostSpeed * ThrottlePercentage; //Consume boost.

                if (AvailableBoost < 0) //Sanity check available boost.
                {
                    AvailableBoost = 0;
                }
            }
            else if (ThrottlePercentage > 0) //Ramp down the boost.
            {
                ThrottlePercentage -= _engine.Settings.EnemyVelocityRampDown;
                if (ThrottlePercentage < 0)
                {
                    ThrottlePercentage = 0;
                }
            }
            */

            base.ApplyMotion(epoch, displacementVector);
        }

        public override void ApplyIntelligence(float epoch, CosVector displacementVector)
        {
            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}
