using SharpDX.DirectWrite;

namespace Cos.Rendering
{
    public class CosPreCreatedTextFormats
    {
        public TextFormat MenuGeneral { get; private set; }
        public TextFormat MenuTitle { get; private set; }
        public TextFormat MenuItem { get; private set; }
        public TextFormat TextInputItem { get; private set; }
        public TextFormat LargeBlocker { get; private set; }
        public TextFormat RealtimePlayerStats { get; private set; }
        public TextFormat Loading { get; private set; }

        public CosPreCreatedTextFormats(Factory factory)
        {
            //Digital-7 Mono

            Loading = new TextFormat(factory, "Consolas", 30) { WordWrapping = WordWrapping.NoWrap };
            LargeBlocker = new TextFormat(factory, "Orbitronio", 50) { WordWrapping = WordWrapping.NoWrap };
            MenuGeneral = new TextFormat(factory, "Consolas", 20) { WordWrapping = WordWrapping.NoWrap };
            MenuTitle = new TextFormat(factory, "Orbitronio", 72) { WordWrapping = WordWrapping.NoWrap };
            MenuItem = new TextFormat(factory, "Consolas", 20) { WordWrapping = WordWrapping.NoWrap };
            RealtimePlayerStats = new TextFormat(factory, "Consolas", 16) { WordWrapping = WordWrapping.NoWrap };
            TextInputItem = new TextFormat(factory, "Consolas", 20) { WordWrapping = WordWrapping.NoWrap, };
        }
    }
}
