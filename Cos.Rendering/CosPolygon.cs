using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cos.Rendering
{
    public class CosPolygon
    {
        public RawVector2[] Vertices { get; }

        public CosPolygon(RawVector2[] vertices)
        {
            Vertices = vertices;
        }

        /// <summary>
        /// Used for polygon visualizers like https://www.wolframalpha.com/input
        /// </summary>
        /// <returns></returns>
        public string Plot()
        {
            var sb = new StringBuilder("plot polygon (");

            foreach (var v in Vertices)
            {
                sb.Append($"({v.X}, {v.Y})");
            }

            sb.Append(')');

            return sb.ToString();
        }


        /// <summary>
        /// Get the bounding rectangle of the polygon.
        /// </summary>
        /// <returns></returns>
        public RawRectangleF GetBounds()
        {
            // Initialize bounds with extreme values
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            // Find minimum and maximum X and Y coordinates of polygon vertices
            foreach (var vertex in Vertices)
            {
                minX = Math.Min(minX, vertex.X);
                minY = Math.Min(minY, vertex.Y);
                maxX = Math.Max(maxX, vertex.X);
                maxY = Math.Max(maxY, vertex.Y);
            }

            // Return the bounding rectangle
            return new RawRectangleF(minX, minY, maxX, maxY);
        }

        public CosPolygon Clip(int width, int height)
        {
            // Clip the polygon to ensure it fits within the bounds of the original bitmap
            var clippedPolygon = new List<RawVector2>();
            foreach (var vertex in Vertices)
            {
                float x = Math.Max(0, Math.Min(vertex.X, width - 1));
                float y = Math.Max(0, Math.Min(vertex.Y, height - 1));
                clippedPolygon.Add(new RawVector2(x, y));
            }
            return new CosPolygon(clippedPolygon.ToArray());
        }
    }
}
