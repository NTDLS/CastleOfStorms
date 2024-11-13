using Cos.Engine.Sprite._Superclass._Root;
using Cos.Library.Mathematics;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using System.Drawing;
using static Cos.Library.CosConstants;

namespace Cos.Engine.Sprite
{
    public class SpriteTextBlock : SpriteBase
    {
        private Size _size = Size.Empty;
        private string _text = string.Empty;

        #region Properties.

        //Non-sprites (e.g. only text) bounds are simple, unlike sprites the text bounds start at X,Y and go to Width/Height.
        public override RectangleF Bounds => new(Location.X, Location.Y, Size.Width, Size.Height);
        public override RawRectangleF RawBounds => new(Location.X, Location.Y, Location.X + Size.Width, Location.Y + Size.Height);
        public override RectangleF RenderBounds => new(RenderLocation.X, RenderLocation.Y, Size.Width, Size.Height);
        public override RawRectangleF RawRenderBounds => new(RenderLocation.X, RenderLocation.Y, RenderLocation.X + Size.Width, RenderLocation.Y + Size.Height);

        public TextFormat Format { get; set; }
        public SolidColorBrush Color { get; private set; }
        public float Height => _size.Height;
        public override Size Size => _size;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                var size = _engine.Rendering.GetTextSize(_text, Format);
                _size = new Size((int)size.Width, (int)size.Height);
            }
        }

        #endregion

        public SpriteTextBlock(EngineCore engine, TextFormat format, SolidColorBrush color, CosVector location, bool isFixedPosition)
            : base(engine)
        {
            RenderScaleOrder = CosRenderScaleOrder.PostScale;
            IsFixedPosition = isFixedPosition;
            Location = new CosVector(location);
            Color = color;

            Format = format;
        }

        public void SetTextAndCenterXY(string text)
        {
            Text = text;
            X = _engine.Display.NaturalScreenSize.Width / 2 - Size.Width / 2;
            Y = _engine.Display.NaturalScreenSize.Height / 2 - Size.Height / 2;
        }

        public void SetTextAndCenterY(string text)
        {
            Text = text;
            Y = _engine.Display.NaturalScreenSize.Height / 2 - Size.Height / 2;
        }

        public void SetTextAndCenterX(string text)
        {
            Text = text;
            X = _engine.Display.NaturalScreenSize.Width / 2 - Size.Width / 2;
        }

        public override void Render(RenderTarget renderTarget)
        {
            if (Visible)
            {
                _engine.Rendering.DrawText(renderTarget,
                    RenderLocation.X,
                    RenderLocation.Y,
                    0, _text ?? string.Empty, Format, Color);
            }
        }
    }
}