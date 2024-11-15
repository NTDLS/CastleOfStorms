using Cos.Library.Sprite;
using System.Drawing;

namespace Cos.Engine.Sprite._Superclass._Root
{
    /// <summary>
    /// Represents a single item that can be rendered to the screen. All on-screen objects are derived from this class.
    /// </summary>
    public partial class SpriteBase : ISprite
    {
        public virtual void Render(SharpDX.Direct2D1.RenderTarget renderTarget)
        {
            if (_isVisible && _image != null)
            {
                DrawImage(renderTarget, _image);

                if (IsHoverHighlighted)
                {
                    _engine.Rendering.DrawRectangle(renderTarget, RawRenderBounds,
                        _engine.Rendering.Materials.Colors.Red, 0, 1, Orientation.RadiansSigned);
                }
                if (IsSelectedHighlighted)
                {
                    _engine.Rendering.DrawRectangle(renderTarget, RawRenderBounds,
                        _engine.Rendering.Materials.Colors.Yellow, 0, 1, Orientation.RadiansSigned);
                }
            }
        }

        public virtual void Render(Graphics dc)
        {
        }

        public void DrawImage(SharpDX.Direct2D1.RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, float? angleRadians = null)
        {
            float angle = (float)(angleRadians == null ? Orientation.RadiansSigned : angleRadians);

            _engine.Rendering.DrawBitmap(renderTarget, bitmap,
                RenderLocation.X - bitmap.Size.Width / 2.0f,
                RenderLocation.Y - bitmap.Size.Height / 2.0f, angle);
        }
    }
}
