using Cos.Library;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cos.Rendering
{
    internal static class CosBitmapFragmenter
    {
        /// <summary>
        /// Generates random fragments based on the size of the input image.
        /// </summary>
        /// <param name="rendering"></param>
        /// <param name="originalBitmap"></param>
        /// <returns></returns>
        public static List<Bitmap> GenerateIrregularFragments(CosRendering rendering, Bitmap originalBitmap)
        {
            int factor = (int)((originalBitmap.Size.Width + originalBitmap.Size.Height) / 16.0f);
            var squareNumbers = new int[] { 81, 64, 49, 36, 25, 16, 9, 4 };
            int countOfFragments = 4;

            foreach (var squareNumber in squareNumbers)
            {
                if (squareNumber <= factor)
                {
                    countOfFragments = squareNumber;
                    break;
                }
            }

            return GenerateIrregularFragments(rendering, originalBitmap, countOfFragments, 8);
        }

        public static List<Bitmap> GenerateIrregularFragments(CosRendering rendering, Bitmap originalBitmap, int countOfFragments, int countOfVertices)
        {
            if (CosUtility.IsSquareNumber(countOfFragments) == false)
            {
                throw new Exception("Parameter countOfFragments of GenerateIrregularFragments() (which is {countOfFragments}), must be a square number.");
            }

            // first break the image into a grid or rows and columns.
            int columns = (int)Math.Sqrt(countOfFragments);
            int rows = columns; // Square grid

            var fragments = new List<Bitmap>();

            int pieceWidth = originalBitmap.PixelSize.Width / columns;
            int pieceHeight = originalBitmap.PixelSize.Height / rows;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    // Calculate piece bounds.
                    int x = col * pieceWidth;
                    int y = row * pieceHeight;
                    var pieceRegion = new RawRectangleF(x, y, x + pieceWidth, y + pieceHeight);

                    //Extract the "cell" of the bitmap for the row and column.
                    var piece = ExtractBitmapRegion(rendering, originalBitmap, pieceRegion);

                    //Extract a random polygon from the square bitmap cell.
                    var fragment = GenerateIrregularFragmentFromPiece(rendering, piece, countOfVertices);

                    fragments.Add(fragment);
                }
            }

            return fragments;
        }

        private static Bitmap GenerateIrregularFragmentFromPiece(CosRendering rendering, Bitmap piece, int countOfVertices)
        {
            var pieceWidth = piece.PixelSize.Width;
            var pieceHeight = piece.PixelSize.Height;

            var vertices = GenerateRandomConvexPolygonVertices(countOfVertices, pieceWidth, pieceHeight);

            return rendering.RenderTargets.Use(o =>
            {
                using (var tmpTarget = new BitmapRenderTarget(o.ScreenRenderTarget, CompatibleRenderTargetOptions.None, new Size2F(pieceWidth, pieceHeight)))
                {
                    tmpTarget.BeginDraw();
                    tmpTarget.Clear(null);

                    using (var geometry = new PathGeometry(tmpTarget.Factory))
                    {
                        using (var sink = geometry.Open())
                        {
                            sink.BeginFigure(vertices[0], FigureBegin.Filled);
                            for (int i = 1; i < vertices.Length; i++)
                            {
                                sink.AddLine(vertices[i]);
                            }
                            sink.EndFigure(FigureEnd.Closed);
                            sink.Close();
                        }

                        // Create a layer to use with the geometry mask.
                        using (var layer = new Layer(tmpTarget))
                        {
                            var layerParameters = new LayerParameters()
                            {
                                ContentBounds = new RawRectangleF(-float.MaxValue, -float.MaxValue, float.MaxValue, float.MaxValue),
                                GeometricMask = geometry,
                                MaskTransform = Matrix3x2.Identity,
                                Opacity = 1.0f,
                                LayerOptions = LayerOptions.None,
                            };

                            // Begin using the layer with the geometry as a mask.
                            tmpTarget.PushLayer(ref layerParameters, layer);

                            // Now draw the bitmap; it will be clipped to the geometry.
                            tmpTarget.DrawBitmap(piece, 1.0f, BitmapInterpolationMode.Linear, new RawRectangleF(0, 0, pieceWidth, pieceHeight));

                            // End using the layer to apply the mask.
                            tmpTarget.PopLayer();
                        }
                    }

                    tmpTarget.EndDraw();

                    return tmpTarget.Bitmap;
                }
            });
        }

        private static Bitmap ExtractBitmapRegion(CosRendering rendering, Bitmap originalBitmap, RawRectangleF region)
        {
            // Calculate the width and height of the region to be extracted.
            var width = (int)(region.Right - region.Left);
            var height = (int)(region.Bottom - region.Top);

            // Create a new temporary render target to draw the extracted region.
            Bitmap newBitmap = rendering.RenderTargets.Use(o =>
            {
                using (var tmpTarget = new BitmapRenderTarget(o.ScreenRenderTarget, CompatibleRenderTargetOptions.None, new Size2F(width, height)))
                {
                    tmpTarget.BeginDraw();

                    // Since we're dealing with rectangles, we don't need to create a complex path.
                    // Instead, directly draw the specified region of the original bitmap onto the temporary render target.
                    var destRect = new RawRectangleF(0, 0, width, height); // Destination rectangle in the temporary bitmap.
                    var sourceRect = new RawRectangleF(region.Left, region.Top, region.Right, region.Bottom); // Source rectangle in the original bitmap.

                    // Draw the bitmap region onto the temporary render target.
                    tmpTarget.DrawBitmap(originalBitmap, destRect, 1.0f, BitmapInterpolationMode.Linear, sourceRect);

                    tmpTarget.EndDraw();

                    return tmpTarget.Bitmap;
                }
            });

            return newBitmap;
        }

        public static RawVector2[] GenerateRandomConvexPolygonVertices(int vertexCount, int maxX, int maxY)
        {
            // Step 1: Generate random points.
            var points = new List<RawVector2>();
            for (int i = 0; i < vertexCount; i++)
            {
                points.Add(new RawVector2(CosRandom.Generator.Next(maxX), CosRandom.Generator.Next(maxY)));
            }

            // Step 2: Compute the centroid.
            var centroid = new RawVector2(
                points.Average(point => point.X),
                points.Average(point => point.Y)
            );

            // Step 3: Sort points by angle relative to centroid.
            points.Sort((a, b) =>
            {
                float angleA = MathF.Atan2(a.Y - centroid.Y, a.X - centroid.X);
                float angleB = MathF.Atan2(b.Y - centroid.Y, b.X - centroid.X);
                return angleA.CompareTo(angleB);
            });

            // Step 4: The sorted points form a convex polygon.
            return points.ToArray();
        }
    }
}
