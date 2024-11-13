using System.Drawing;

namespace Cos.Library.Mathematics
{
    public class CosQuadrant
    {
        public Point Key { get; private set; }
        public Rectangle Bounds { get; private set; }

        public CosQuadrant(Point key, Rectangle bounds)
        {
            Key = key;
            Bounds = bounds;
        }

        public override string ToString() => Key.ToString();
    }
}