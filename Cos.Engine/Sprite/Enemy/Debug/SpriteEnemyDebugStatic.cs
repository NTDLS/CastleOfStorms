using Cos.Engine.Sprite.Enemy.Peon._Superclass;

namespace Cos.Engine.Sprite.Enemy.Debug
{
    /// <summary>
    /// Debugging enemy unit - a scary sight to see.
    /// </summary>
    internal class SpriteEnemyDebugStatic : SpriteEnemyPeonBase
    {
        public SpriteEnemyDebugStatic(EngineCore engine)
            : base(engine, @"Sprites\Enemy\Debug\Hull.png")
        {
        }
    }
}
