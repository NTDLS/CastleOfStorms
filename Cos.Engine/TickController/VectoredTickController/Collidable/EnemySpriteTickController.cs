using Cos.Engine;
using Cos.Engine.Manager;
using Cos.Engine.Sprite.Enemy._Superclass;
using Cos.Engine.TickController._Superclass;
using Cos.Library;
using Cos.Library.Mathematics;
using NTDLS.Helpers;
using System;

namespace Cos.GameEngine.TickController.VectoredTickController.Collidable
{
    public class EnemySpriteTickController : VectoredCollidableTickControllerBase<SpriteEnemyBase>
    {
        private readonly EngineCore _engine;

        public EnemySpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
            _engine = engine;
        }

        public override void ExecuteWorldClockTick(float epoch, CosVector displacementVector)
        {
            foreach (var enemy in Visible())
            {
                enemy.ApplyIntelligence(epoch, displacementVector);
                enemy.ApplyMotion(epoch, displacementVector);
                enemy.RenewableResources.RenewAllResources(epoch);
            }
        }

        public T AddTypeOf<T>() where T : SpriteEnemyBase
        {
            object[] param = { Engine };
            SpriteEnemyBase obj = (SpriteEnemyBase)Activator.CreateInstance(typeof(T), param).EnsureNotNull();

            obj.Location = Engine.Display.RandomOffScreenLocation();
            obj.Orientation.Degrees = CosRandom.Between(0, 359);
            obj.RecalculateMovementVector();

            obj.BeforeCreate();
            SpriteManager.Add(obj);
            obj.AfterCreate();

            return (T)obj;
        }
    }
}
