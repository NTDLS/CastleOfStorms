using Cos.Library;
using Cos.Library.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Cos.Engine.Manager
{
    /// <summary>
    /// Various metrics related to display.
    /// </summary>
    public class DisplayManager
    {
        private readonly EngineCore _engine;

        public CosFrameCounter FrameCounter { get; private set; } = new();

        public Dictionary<Point, CosQuadrant> Quadrants { get; private set; } = new();

        /// <summary>
        /// The X,Y of the top left of the render window. This is the corner of the total
        /// canvas which includes offscreen locations when not zoomed out. The local player
        /// will be centered in this window and the window will moved with the players movements.
        /// This can be though of as the camera.
        /// </summary>
        public CosVector RenderWindowPosition { get; set; } = new();
        public Control DrawingSurface { get; private set; }
        public Screen Screen { get; private set; }

        public bool IsDrawingSurfaceFocused { get; set; } = false;
        public void SetIsDrawingSurfaceFocused(bool isFocused) => IsDrawingSurfaceFocused = isFocused;


        /// <summary>
        /// The total size of the rendering surface (no scaling).
        /// </summary>
        public Size CanvasSize { get; private set; }

        public float TotalCanvasDiagonal { get; private set; }

        public CosVector CenterCanvas;
        public CosVector CenterOfCurrentScreen => RenderWindowPosition + CenterCanvas;

        /// <summary>
        /// The total bounds of the drawing surface (canvas) natural + overdraw (with no scaling).
        /// </summary>
        public RectangleF TotalCanvasBounds => new RectangleF(0, 0, CanvasSize.Width, CanvasSize.Height);

        public RectangleF GetCurrentScaledScreenBounds()
        {
            float centerX = CanvasSize.Width * 0.5f;
            float centerY = CanvasSize.Height * 0.5f;

            float left = centerX - CanvasSize.Width * 0.5f;
            float top = centerY - CanvasSize.Height * 0.5f;
            float right = CanvasSize.Width;
            float bottom = CanvasSize.Height;

            return new RectangleF(left, top, right, bottom);

        }

        public CosVector RandomOnScreenLocation()
        {
            var currentScaledScreenBounds = GetCurrentScaledScreenBounds();

            return new CosVector(
                    CosRandom.Between((int)currentScaledScreenBounds.Left, (int)(currentScaledScreenBounds.Left + currentScaledScreenBounds.Width)),
                    CosRandom.Between((int)currentScaledScreenBounds.Top, (int)(currentScaledScreenBounds.Top + currentScaledScreenBounds.Height))
                );
        }

        //TODO: Test and fix this.
        public CosVector RandomOffScreenLocation(int minOffscreenDistance = 100, int maxOffscreenDistance = 500)
        {
            if (CosRandom.FlipCoin())
            {
                if (CosRandom.FlipCoin())
                {
                    return new CosVector(
                        RenderWindowPosition.X + -CosRandom.Between(minOffscreenDistance, maxOffscreenDistance),
                        RenderWindowPosition.Y + CosRandom.Between(0, CanvasSize.Height));
                }
                else
                {
                    return new CosVector(
                        RenderWindowPosition.X + CosRandom.Between(minOffscreenDistance, maxOffscreenDistance),
                        RenderWindowPosition.Y + CosRandom.Between(0, CanvasSize.Height));
                }
            }
            else
            {
                if (CosRandom.FlipCoin())
                {
                    return new CosVector(
                        RenderWindowPosition.X + CanvasSize.Width + CosRandom.Between(minOffscreenDistance, maxOffscreenDistance),
                        RenderWindowPosition.Y + CosRandom.Between(0, CanvasSize.Height));
                }
                else
                {
                    return new CosVector(
                        RenderWindowPosition.X + CanvasSize.Width + CosRandom.Between(minOffscreenDistance, maxOffscreenDistance),
                        RenderWindowPosition.Y + -CosRandom.Between(0, CanvasSize.Height));
                }
            }
        }

        public DisplayManager(EngineCore engine, Control drawingSurface)
        {
            _engine = engine;
            DrawingSurface = drawingSurface;

            Screen = Screen.FromHandle(drawingSurface.Handle);

            CanvasSize = new Size(drawingSurface.Width, drawingSurface.Height);
            CenterCanvas = new CosVector(CanvasSize.Width / 2.0f, CanvasSize.Height / 2.0f);

            TotalCanvasDiagonal = (float)Math.Sqrt(CanvasSize.Width * CanvasSize.Width + CanvasSize.Height * CanvasSize.Height);
        }
    }
}
