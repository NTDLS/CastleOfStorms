using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace Cos.Rendering
{
    public class CosPreCreatedMaterials
    {
        public class RawColors
        {
            public RawColor4 Red { get; private set; } = new(1, 0, 0, 1);
            public RawColor4 Green { get; private set; } = new(0, 1, 0, 1);
            public RawColor4 Blue { get; private set; } = new(0, 0, 1, 1);
            public RawColor4 Black { get; private set; } = new(0, 0, 0, 1);
            public RawColor4 White { get; private set; } = new(1, 1, 1, 1);
            public RawColor4 EditorBackground { get; private set; } = new(30 / 255.0f, 50 / 255.0f, 40 / 255.0f, 1);
            public RawColor4 Gray { get; private set; } = new(0.25f, 0.25f, 0.25f, 1);
            public RawColor4 WhiteSmoke { get; private set; } = new(0.9608f, 0.9608f, 0.9608f, 1);
            public RawColor4 Cyan { get; private set; } = new(0, 1f, 1f, 1f);
            public RawColor4 OrangeRed { get; private set; } = new(1f, 0.2706f, 0.0000f, 1);
            public RawColor4 Orange { get; private set; } = new(1.0f, 0.6471f, 0.0f, 1);
            public RawColor4 LawnGreen { get; private set; } = new(0.4863f, 0.9882f, 0f, 1);
            public RawColor4 Transparent { get; private set; } = new(0, 0, 0, 0);
        }

        public class ColorBrushes
        {
            public SolidColorBrush Red { get; internal set; }
            public SolidColorBrush Green { get; internal set; }
            public SolidColorBrush Blue { get; internal set; }
            public SolidColorBrush Black { get; internal set; }
            public SolidColorBrush White { get; internal set; }
            public SolidColorBrush Gray { get; internal set; }
            public SolidColorBrush WhiteSmoke { get; internal set; }
            public SolidColorBrush Cyan { get; internal set; }
            public SolidColorBrush OrangeRed { get; internal set; }
            public SolidColorBrush Orange { get; internal set; }
            public SolidColorBrush LawnGreen { get; internal set; }
            public SolidColorBrush Transparent { get; internal set; }

            public ColorBrushes(RenderTarget renterTarget, RawColors color)
            {
                Red = new SolidColorBrush(renterTarget, color.Red);
                Green = new SolidColorBrush(renterTarget, color.Green);
                Blue = new SolidColorBrush(renterTarget, color.Blue);
                Black = new SolidColorBrush(renterTarget, color.Black);
                White = new SolidColorBrush(renterTarget, color.White);
                Gray = new SolidColorBrush(renterTarget, color.Gray);
                WhiteSmoke = new SolidColorBrush(renterTarget, color.WhiteSmoke);
                Cyan = new SolidColorBrush(renterTarget, color.Cyan);
                OrangeRed = new SolidColorBrush(renterTarget, color.OrangeRed);
                Orange = new SolidColorBrush(renterTarget, color.Orange);
                LawnGreen = new SolidColorBrush(renterTarget, color.LawnGreen);
                Transparent = new SolidColorBrush(renterTarget, color.Transparent);
            }
        }

        public ColorBrushes Brushes { get; private set; }
        public RawColors Colors { get; private set; } = new();

        internal CosPreCreatedMaterials(RenderTarget renterTarget)
        {
            Brushes = new ColorBrushes(renterTarget, Colors);
        }
    }
}
