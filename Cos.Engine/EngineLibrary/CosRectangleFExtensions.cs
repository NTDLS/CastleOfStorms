using SharpDX.Mathematics.Interop;
using System.Drawing;

namespace Cos.Engine.EngineLibrary
{
    public static class CosRectangleFExtensions
    {
        public static RawRectangleF ToRawRectangleF(this RectangleF rectangle)
        {
            return new RawRectangleF(
                        rectangle.X, rectangle.Y,
                        rectangle.X + rectangle.Width,
                        rectangle.Y + rectangle.Height);
        }
    }
}
