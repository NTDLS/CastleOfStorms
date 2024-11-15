using Cos.Engine.Sprite.Enemy._Superclass;

namespace Cos.Engine.Sprite.Enemy.Peon._Superclass
{
    /// <summary>
    /// Base class for "Peon" enemies. These guys are basically all the same in their functionality and animations.
    /// </summary>
    internal class SpriteEnemyPeonBase : SpriteEnemyBase
    {
        public SpriteEnemyPeonBase(EngineCore engine, string imagePath)
            : base(engine, imagePath)
        {
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }
    }
}
