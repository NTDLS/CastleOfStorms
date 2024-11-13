using Cos.Engine;
using Cos.Engine.Manager;
using Cos.Engine.Sprite;
using Cos.Engine.TickController._Superclass;
using Cos.Library.Mathematics;

namespace Cos.GameEngine.TickController.VectoredTickController.Collidable
{
    /// <summary>
    /// These are just minimal non-collidable, non interactive, generic bitmap sprites.
    /// </summary>
    public class MinimalBitmapSpriteTickController : VectoredCollidableTickControllerBase<SpriteMinimalBitmap>
    {
        public MinimalBitmapSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, CosVector displacementVector)
        {
            foreach (var sprite in Visible())
            {
                sprite.ApplyMotion(epoch, displacementVector);
            }
        }
    }
}
