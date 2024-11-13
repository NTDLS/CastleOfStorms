using Cos.Engine;
using Cos.Engine.Manager;
using Cos.Engine.Sprite;
using Cos.Engine.TickController._Superclass;
using Cos.Library.Mathematics;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System.Linq;

namespace Cos.GameEngine.TickController.VectoredTickController.Uncollidable
{
    public class TextBlocksSpriteTickController : VectoredTickControllerBase<SpriteTextBlock>
    {
        public SpriteTextBlock PlayerStatsText { get; private set; }
        public SpriteTextBlock DebugText { get; private set; }
        public SpriteTextBlock PausedText { get; private set; }


        public TextBlocksSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
            PlayerStatsText = Add(engine.Rendering.TextFormats.RealtimePlayerStats, engine.Rendering.Materials.Brushes.WhiteSmoke, new CosVector(5, 5), true);
            PlayerStatsText.Visible = false;
            DebugText = Add(engine.Rendering.TextFormats.RealtimePlayerStats, engine.Rendering.Materials.Brushes.Cyan, new CosVector(5, PlayerStatsText.Y + 100), true);

            //We have to create this ahead of time because we cant create pause text when paused since sprites are created via events.
            PausedText = Add(engine.Rendering.TextFormats.LargeBlocker,
                    engine.Rendering.Materials.Brushes.Red, new CosVector(100, 100), true, "PausedText", "Paused");

            PausedText.Visible = false;
        }

        public override void ExecuteWorldClockTick(float epoch, CosVector displacementVector)
        {
            foreach (var textBlock in Visible().Where(o => o.IsFixedPosition == false))
            {
                textBlock.ApplyMotion(epoch, displacementVector);
            }
        }

        #region Factories.

        public SpriteTextBlock Add(TextFormat format, SolidColorBrush color, CosVector location, bool isPositionStatic)
        {
            var obj = new SpriteTextBlock(Engine, format, color, location, isPositionStatic);
            SpriteManager.Add(obj);
            return obj;
        }

        public SpriteTextBlock Add(TextFormat format, SolidColorBrush color, CosVector location, bool isPositionStatic, string name)
        {
            var obj = new SpriteTextBlock(Engine, format, color, location, isPositionStatic);
            obj.SpriteTag = name;
            SpriteManager.Add(obj);
            return obj;
        }

        public SpriteTextBlock Add(TextFormat format, SolidColorBrush color, CosVector location, bool isPositionStatic, string name, string text)
        {
            var obj = new SpriteTextBlock(Engine, format, color, location, isPositionStatic);
            obj.SpriteTag = name;
            obj.Text = text;
            SpriteManager.Add(obj);
            return obj;
        }

        #endregion
    }
}
