using SharpDX.Direct2D1;

namespace Cos.Rendering
{
    public class CosCriticalRenderTargets
    {
        public BitmapRenderTarget? IntermediateRenderTarget { get; set; }
        public WindowRenderTarget? ScreenRenderTarget { get; set; }
    }
}
