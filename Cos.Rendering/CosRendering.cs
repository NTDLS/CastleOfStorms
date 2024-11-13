using Cos.Library;
using NTDLS.Semaphore;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Cos.Rendering
{
    public class CosRendering : IDisposable
    {
        private struct ScreenShake
        {
            public float Intensity = 0;
            public double Duration = 0;
            public Stopwatch Timer = new();
            public ScreenShake() { }
        }

        public PessimisticCriticalResource<CosCriticalRenderTargets> RenderTargets { get; private set; } = new();
        public CosPreCreatedMaterials Materials { get; private set; }
        public CosPreCreatedTextFormats TextFormats { get; private set; }

        private readonly List<ScreenShake> _screenShakes = new();
        private readonly SharpDX.Direct2D1.Factory _direct2dFactory = new(FactoryType.SingleThreaded);
        private readonly SharpDX.DirectWrite.Factory _directWriteFactory = new();
        private readonly ImagingFactory _wicFactory = new();
        private Size _totalCanvasSize;
        private Size _drawingSurfaceSize;

        public CosRendering(CosEngineSettings settings, Control drawingSurface, Size canvasSize)
        {
            _drawingSurfaceSize = drawingSurface.Size;
            _totalCanvasSize = canvasSize;

            var presentOptions = PresentOptions.Immediately;
            var antiAliasMode = AntialiasMode.Aliased;

            if (settings.VerticalSync == true)
            {
                presentOptions = PresentOptions.None;
            }

            if (settings.AntiAliasing == true)
            {
                antiAliasMode = AntialiasMode.PerPrimitive;
            }

            var windowRenderProperties = new HwndRenderTargetProperties()
            {
                PresentOptions = presentOptions,
                Hwnd = drawingSurface.Handle,
                PixelSize = new Size2(_drawingSurfaceSize.Width, _drawingSurfaceSize.Height)
                //PixelSize = new Size2(engine.Display.NaturalScreenSize.Width, engine.Display.NaturalScreenSize.Height)
            };

            var renderTargetProperties = new RenderTargetProperties
            {
                PixelFormat = new SharpDX.Direct2D1.PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied),
                //MinLevel = FeatureLevel.Level_10,
                Type = RenderTargetType.Hardware
            };

            //The intermediate render target is much larger than the render target window. We create this
            //  larger render target so that we can zoom-out when we want to see more of the universe.
            var intermediateRenderTargetSize = new Size2F(_totalCanvasSize.Width, _totalCanvasSize.Height);

            var renderTargets = new CosCriticalRenderTargets()
            {
                ScreenRenderTarget = new WindowRenderTarget(_direct2dFactory, renderTargetProperties, windowRenderProperties)
                {
                    AntialiasMode = antiAliasMode
                }
            };

            renderTargets.IntermediateRenderTarget = new BitmapRenderTarget(
                renderTargets.ScreenRenderTarget, CompatibleRenderTargetOptions.None, intermediateRenderTargetSize)
            {
                AntialiasMode = antiAliasMode
            };

            RenderTargets.Use(o =>
            {
                o.ScreenRenderTarget = renderTargets.ScreenRenderTarget;
                o.IntermediateRenderTarget = renderTargets.IntermediateRenderTarget;
            });

            Materials = new CosPreCreatedMaterials(renderTargets.ScreenRenderTarget);
            TextFormats = new CosPreCreatedTextFormats(_directWriteFactory);

            CosTransforms.RegisterRenderTarget(renderTargets.ScreenRenderTarget);
            CosTransforms.RegisterRenderTarget(renderTargets.IntermediateRenderTarget);
        }

        public void Dispose()
        {
            RenderTargets.Use(o =>
            {
                o.ScreenRenderTarget?.Dispose();
                o.ScreenRenderTarget?.Dispose();
            });
        }

        /// <summary>
        /// Transfers the image from one render target to the other with a scaling factor.
        /// Also applies screen shake since this is just a hella convenient place to do it.
        /// </summary>
        /// <param name="intermediateRenderTarget"></param>
        /// <param name="screenRenderTarget"></param>
        /// <param name="scale"></param>
        public void TransferWithZoom(BitmapRenderTarget intermediateRenderTarget, RenderTarget screenRenderTarget, float scale)
        {
            var sourceRect = CosRenderingUtility.CalculateCenterCopyRectangle(intermediateRenderTarget.Size, scale);
            var destRect = new RawRectangleF(0, 0, _drawingSurfaceSize.Width, _drawingSurfaceSize.Height);

            var appliedScreenShakes = new List<ScreenShake>();

            foreach (var screenShake in _screenShakes)
            {
                var totalElapsedScreenShakeTime = ((double)screenShake.Timer.ElapsedTicks / (double)Stopwatch.Frequency) * 1000.0;

                var percentComplete = (float)(totalElapsedScreenShakeTime / screenShake.Duration);
                if (percentComplete >= 1)
                {
                    screenShake.Timer.Stop();
                }

                var intensity = screenShake.Intensity * (1 - percentComplete);

                var offsetX = (float)(CosRandom.NextFloat() * intensity * 2 - intensity);
                var offsetY = (float)(CosRandom.NextFloat() * intensity * 2 - intensity);

                CosTransforms.PushTransform(screenRenderTarget, Matrix3x2.Translation(offsetX, offsetY));

                appliedScreenShakes.Add(screenShake);
            }

            screenRenderTarget.DrawBitmap(intermediateRenderTarget.Bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear, sourceRect);

            foreach (var screenShake in appliedScreenShakes)
            {
                if (screenShake.Timer.IsRunning == false)
                {
                    _screenShakes.Remove(screenShake);
                }

                CosTransforms.PopTransform(screenRenderTarget);
            }
        }

        #region Rending: Bitmaps.

        /// <summary>
        /// Draws a bitmap at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the bitmap.</returns>
        public void DrawBitmap(RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, float x, float y, float angleRadians)
        {
            if (angleRadians > 6.3)
            {
                //throw new Exception($"Radians are out of range: {angleRadians:n4}");
            }

            var destRect = new RawRectangleF(x, y, (x + bitmap.PixelSize.Width), (y + bitmap.PixelSize.Height));
            CosTransforms.PushTransform(renderTarget, CosTransforms.CreateAngleTransform(destRect, angleRadians));
            renderTarget.DrawBitmap(bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
            CosTransforms.PopTransform(renderTarget);
        }

        /// Draws a bitmap from a specified location of a given size, to the the specified location.
        public void DrawBitmap(RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap,
            float x, float y, float angleRadians, RawRectangleF sourceRect, Size2F destSize)
        {
            if (angleRadians > 6.3)
            {
                //throw new Exception($"Radians are out of range: {angleRadians:n4}");
            }

            var destRect = new RawRectangleF(x, y, (x + destSize.Width), (y + destSize.Height));
            CosTransforms.PushTransform(renderTarget, CosTransforms.CreateAngleTransform(destRect, angleRadians));
            renderTarget.DrawBitmap(bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear, sourceRect);
            CosTransforms.PopTransform(renderTarget);
        }

        /// <summary>
        /// Draws a bitmap at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the bitmap.</returns>
        public void DrawBitmap(RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, float x, float y)
        {
            var destRect = new RawRectangleF(x, y, (x + bitmap.PixelSize.Width), (y + bitmap.PixelSize.Height));
            renderTarget.DrawBitmap(bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
        }

        #endregion

        #region Rending: Text.

        public RawRectangleF GetTextRect(float x, float y, string text, SharpDX.DirectWrite.TextFormat format)
        {
            using var textLayout = new SharpDX.DirectWrite.TextLayout(_directWriteFactory, text, format, float.MaxValue, float.MaxValue);
            return new RawRectangleF(x, y, (x + textLayout.Metrics.Width), (y + textLayout.Metrics.Height));
        }

        public SizeF GetTextSize(string text, SharpDX.DirectWrite.TextFormat format)
        {
            //We have to check the size with some ending characters because TextLayout() seems to want to trim the text before calculating the metrics.
            using var textLayout = new SharpDX.DirectWrite.TextLayout(_directWriteFactory, $"[{text}]", format, float.MaxValue, float.MaxValue);
            using var spacerLayout = new SharpDX.DirectWrite.TextLayout(_directWriteFactory, "[]", format, float.MaxValue, float.MaxValue);
            return new SizeF(textLayout.Metrics.Width - spacerLayout.Metrics.Width, textLayout.Metrics.Height);
        }

        /// <summary>
        /// Draws text at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the text.</returns>
        public void DrawText(RenderTarget renderTarget,
            float x, float y, float angleRadians, string text, SharpDX.DirectWrite.TextFormat format, SolidColorBrush brush)
        {
            using var textLayout = new SharpDX.DirectWrite.TextLayout(_directWriteFactory, text, format, float.MaxValue, float.MaxValue);

            var textWidth = textLayout.Metrics.Width;
            var textHeight = textLayout.Metrics.Height;

            // Create a rectangle that fits the text
            var destRect = new RawRectangleF(x, y, (x + textWidth), (y + textHeight));

            //DrawRectangle(renderTarget, textRectangle, 0, Materials.Raw.Blue, 0, 1);

            CosTransforms.PushTransform(renderTarget, CosTransforms.CreateAngleTransform(destRect, angleRadians));
            renderTarget.DrawText(text, format, destRect, brush);
            CosTransforms.PopTransform(renderTarget);
        }

        #endregion

        #region Rending: Lines.

        public void DrawLine(RenderTarget renderTarget,
            float startPointX, float startPointY, float endPointX, float endPointY,
            SolidColorBrush brush, float strokeWidth = 1)
        {
            var startPoint = new RawVector2(startPointX, startPointY);
            var endPoint = new RawVector2(endPointX, endPointY);

            renderTarget.DrawLine(startPoint, endPoint, brush, strokeWidth);
        }

        #endregion

        #region Rending: Ellipse.

        /// <summary>
        /// Draws a color filled ellipse at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public void DrawSolidEllipse(RenderTarget renderTarget, float x, float y,
            float radiusX, float radiusY, Color4 color, float angleRadians = 0)
        {
            var ellipse = new Ellipse()
            {
                Point = new RawVector2(x, y),
                RadiusX = radiusX,
                RadiusY = radiusY,
            };

            var destRect = new RawRectangleF(
                (x - radiusX / 2.0f),
                (y - radiusY / 2.0f),
                ((x - radiusX / 2.0f) + radiusX),
                ((y - radiusY / 2.0f) + radiusY));
            CosTransforms.PushTransform(renderTarget, CosTransforms.CreateAngleTransform(destRect, angleRadians));

            using var brush = new SolidColorBrush(renderTarget, color);
            renderTarget.FillEllipse(ellipse, brush);

            CosTransforms.PopTransform(renderTarget);
        }

        /// <summary>
        /// Draws a color gradient filled ellipse at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public void DrawGradientEllipse(RenderTarget renderTarget, float x, float y,
            float radiusX, float radiusY, Color4 startColor, Color4 endColor, float angleRadians = 0)
        {
            var ellipse = new Ellipse()
            {
                Point = new RawVector2(x, y),
                RadiusX = radiusX,
                RadiusY = radiusY,
            };

            var destRect = new RawRectangleF(
                (x - radiusX / 2.0f),
                (y - radiusY / 2.0f),
                ((x - radiusX / 2.0f) + radiusX),
                ((y - radiusY / 2.0f) + radiusY));
            CosTransforms.PushTransform(renderTarget, CosTransforms.CreateAngleTransform(destRect, angleRadians));

            // Define gradient stops
            using var gradientStops = new GradientStopCollection(renderTarget, new GradientStop[]
            {
                new GradientStop() { Position = 0.0f, Color = startColor },
                new GradientStop() { Position = 1.0f, Color = endColor }
            });

            // Create linear gradient brush
            using var linearGradientBrush = new LinearGradientBrush(renderTarget,
                new LinearGradientBrushProperties()
                {
                    StartPoint = new RawVector2(x - radiusX, y),
                    EndPoint = new RawVector2(x + radiusX, y)
                }, gradientStops);

            // Fill ellipse with gradient brush
            renderTarget.FillEllipse(ellipse, linearGradientBrush);

            CosTransforms.PopTransform(renderTarget);
        }

        /// <summary>
        /// Draws a hollow ellipse at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public void DrawEllipse(RenderTarget renderTarget, float x, float y,
            float radiusX, float radiusY, Color4 color, float strokeWidth = 1, float angleRadians = 0)
        {
            var ellipse = new Ellipse()
            {
                Point = new RawVector2(x, y),
                RadiusX = radiusX,
                RadiusY = radiusY,
            };

            var destRect = new RawRectangleF(
                (x - radiusX / 2.0f),
                (y - radiusY / 2.0f),
                ((x - radiusX / 2.0f) + radiusX),
                ((y - radiusY / 2.0f) + radiusY));

            CosTransforms.PushTransform(renderTarget, CosTransforms.CreateAngleTransform(destRect, angleRadians));
            using var brush = new SolidColorBrush(renderTarget, color);
            renderTarget.DrawEllipse(ellipse, brush, strokeWidth);

            CosTransforms.PopTransform(renderTarget);
        }

        #endregion

        #region Rending: Triangle.

        public void DrawTriangle(RenderTarget renderTarget, float x, float y,
            float height, float width, Color4 color, float strokeWidth = 1, float angleRadians = 0)
        {
            // Define the points for the triangle
            var trianglePoints = new RawVector2[]
            {
                new RawVector2(0, height),           // Vertex 1 (bottom-left)
                new RawVector2(width, height), // Vertex 2 (bottom-right)
                new RawVector2((width / 2.0f), 0)      // Vertex 3 (top-center)
            };

            // Create a PathGeometry and add the triangle to it
            var triangleGeometry = new PathGeometry(_direct2dFactory);
            using (GeometrySink sink = triangleGeometry.Open())
            {
                sink.BeginFigure(trianglePoints[0], FigureBegin.Filled);
                sink.AddLines(trianglePoints);
                sink.EndFigure(FigureEnd.Closed);
                sink.Close();
            }

            // Calculate the center of the triangle
            float centerX = (trianglePoints[0].X + trianglePoints[1].X + trianglePoints[2].X) / 3;
            float centerY = (trianglePoints[0].Y + trianglePoints[1].Y + trianglePoints[2].Y) / 3;

            // Calculate the adjustment needed to center the triangle at the desired position
            x -= centerX;
            y -= centerY;

            // Create a translation transform to move the triangle to the desired position
            var destRect = new RawRectangleF(x, y, (x + width), (y + height));

            CosTransforms.PushTransform(renderTarget,
                Matrix3x2.Multiply(CosTransforms.CreateOffsetTransform(x, y), CosTransforms.CreateAngleTransform(destRect, angleRadians)));

            using var brush = new SolidColorBrush(renderTarget, color);
            renderTarget.DrawGeometry(triangleGeometry, brush, strokeWidth);

            CosTransforms.PopTransform(renderTarget);
        }

        #endregion

        #region Rending: Polygon.

        public void DrawPolygon(RenderTarget renderTarget, PointF[] points, RawColor4 color, float strokeWidth = 1.0f)
        {
            DrawPolygon(renderTarget, 0, 0, points, color, strokeWidth);
        }

        public void DrawPolygon(RenderTarget renderTarget, float x, float y, PointF[] points, RawColor4 color, float strokeWidth = 1.0f)
        {
            if (points.Count() == 0)
            {
                return;
            }
            var rawPoints = Array.ConvertAll(points, point => new RawVector2(point.X + x, point.Y + y));

            // Create a PathGeometry to define the shape of the polygon
            using (var pathGeometry = new PathGeometry(_direct2dFactory))
            {
                using (var geometrySink = pathGeometry.Open())
                {
                    geometrySink.BeginFigure(rawPoints[0], FigureBegin.Filled);
                    geometrySink.AddLines(rawPoints);
                    geometrySink.EndFigure(FigureEnd.Closed);
                    geometrySink.Close();
                }

                // Draw the polygon
                using (var brush = new SolidColorBrush(renderTarget, color))
                {
                    renderTarget.DrawGeometry(pathGeometry, brush, strokeWidth);
                }
            }
        }

        #endregion

        #region Rending: Rectangle.

        /// <summary>
        /// Draws a rectangle at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public RawRectangleF DrawRectangle(RenderTarget renderTarget, RawRectangleF destRect,
            RawColor4 color, float expand = 0, float strokeWidth = 1, float angleRadians = 0)
            => DrawRectangle(renderTarget, 0, 0, destRect, color, expand, strokeWidth, angleRadians);

        /// <summary>
        /// Draws a rectangle at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public RawRectangleF DrawRectangle(RenderTarget renderTarget, float x, float y, RawRectangleF destRect,
            Color4 color, float expand = 0, float strokeWidth = 1, float angleRadians = 0)
        {
            if (expand != 0)
            {
                destRect.Left -= expand;
                destRect.Top -= expand;
                destRect.Bottom += expand;
                destRect.Right += expand;
            }

            if (x != 0 && y != 0)
            {
                destRect.Left += x;
                destRect.Top += y;
                destRect.Bottom += y;
                destRect.Right += x;
            }

            CosTransforms.PushTransform(renderTarget, CosTransforms.CreateAngleTransform(destRect, angleRadians));
            using var brush = new SolidColorBrush(renderTarget, color);
            renderTarget.DrawRectangle(destRect, brush, strokeWidth);
            CosTransforms.PopTransform(renderTarget);

            return destRect;
        }

        /// <summary>
        /// Draws a rectangle at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public RawRectangleF DrawSolidRectangle(RenderTarget renderTarget, RawRectangleF destRect,
            RawColor4 color, float expand = 0, float angleRadians = 0)
            => DrawSolidRectangle(renderTarget, 0, 0, destRect, color, expand, angleRadians);

        public RawRectangleF DrawSolidRectangle(RenderTarget renderTarget, float x, float y, RawRectangleF destRect,
            Color4 color, float expand = 0, float angleRadians = 0)
        {
            if (expand != 0)
            {
                destRect.Left -= expand;
                destRect.Top -= expand;
                destRect.Bottom += expand;
                destRect.Right += expand;
            }

            if (x != 0 && y != 0)
            {
                destRect.Left += x;
                destRect.Top += y;
                destRect.Bottom += y;
                destRect.Right += x;
            }

            CosTransforms.PushTransform(renderTarget, CosTransforms.CreateAngleTransform(destRect, angleRadians));
            using var brush = new SolidColorBrush(renderTarget, color);
            renderTarget.FillRectangle(destRect, brush);
            CosTransforms.PopTransform(renderTarget);

            return destRect;
        }

        /// <summary>
        /// Draws a rectangle at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public RawRectangleF DrawGradientRectangle(RenderTarget renderTarget, RawRectangleF destRect,
            Color4 startColor, Color4 endColor, float expand = 0, float angleRadians = 0)
            => DrawGradientRectangle(renderTarget, 0, 0, destRect, startColor, endColor, expand, angleRadians);

        public RawRectangleF DrawGradientRectangle(RenderTarget renderTarget, float x, float y, RawRectangleF destRect,
            Color4 startColor, Color4 endColor, float expand = 0, float angleRadians = 0)
        {
            if (expand != 0)
            {
                destRect.Left -= expand;
                destRect.Top -= expand;
                destRect.Bottom += expand;
                destRect.Right += expand;
            }

            if (x != 0 && y != 0)
            {
                destRect.Left += x;
                destRect.Top += y;
                destRect.Bottom += y;
                destRect.Right += x;
            }

            CosTransforms.PushTransform(renderTarget, CosTransforms.CreateAngleTransform(destRect, angleRadians));

            // Define start and end points for the gradient
            var startPoint = new RawVector2(destRect.Left, destRect.Top);
            var endPoint = new RawVector2(destRect.Left, destRect.Bottom);

            // Create gradient stops
            var gradientStops = new GradientStop[]
            {
                new GradientStop { Color = startColor, Position = 0.0f },
                new GradientStop { Color = endColor, Position = 1.0f }
            };

            using (var gradientStopCollection = new GradientStopCollection(renderTarget, gradientStops, Gamma.Linear, ExtendMode.Clamp))

            using (var linearGradientBrush = new LinearGradientBrush(renderTarget,
                new LinearGradientBrushProperties
                {
                    StartPoint = startPoint,
                    EndPoint = endPoint
                }, gradientStopCollection))

                renderTarget.FillRectangle(destRect, linearGradientBrush);
            CosTransforms.PopTransform(renderTarget);

            return destRect;
        }

        #endregion

        public List<SharpDX.Direct2D1.Bitmap> GenerateIrregularFragments(SharpDX.Direct2D1.Bitmap originalBitmap, int countOfFragments, int countOfVertices = 8)
            => CosBitmapFragmenter.GenerateIrregularFragments(this, originalBitmap, countOfFragments, countOfVertices);

        public List<SharpDX.Direct2D1.Bitmap> GenerateIrregularFragments(SharpDX.Direct2D1.Bitmap originalBitmap)
            => CosBitmapFragmenter.GenerateIrregularFragments(this, originalBitmap);

        public void AddScreenShake(float intensity, float duration)
        {
            var screenShake = new ScreenShake
            {
                Intensity = intensity,
                Duration = duration,
            };

            screenShake.Timer.Start();
            _screenShakes.Add(screenShake);
        }

        public SharpDX.Direct2D1.Bitmap BitmapStreamToD2DBitmap(Stream stream)
        {
            using var decoder = new BitmapDecoder(_wicFactory, stream, DecodeOptions.CacheOnLoad);
            using var frame = decoder.GetFrame(0);
            using var converter = new FormatConverter(_wicFactory);

            converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppPBGRA);

            return RenderTargets.Use(o => SharpDX.Direct2D1.Bitmap.FromWicBitmap(o.ScreenRenderTarget, converter));
        }
    }
}
