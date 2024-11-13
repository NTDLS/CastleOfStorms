using Cos.Engine;
using Cos.Engine.Manager;
using Cos.Engine.Sprite;
using Cos.Engine.TickController._Superclass;
using Cos.Library.Mathematics;

namespace Cos.GameEngine.TickController.VectoredTickController.Collidable
{
    public class DebugSpriteTickController : VectoredCollidableTickControllerBase<SpriteDebug>
    {
        public DebugSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, CosVector displacementVector)
        {
            foreach (var sprite in Visible())
            {
                sprite.ApplyIntelligence(epoch, displacementVector);
                sprite.ApplyMotion(epoch, displacementVector);
            }
        }
    }
}
