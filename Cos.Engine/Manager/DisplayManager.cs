using Cos.Library;
using Cos.Library.ExtensionMethods;
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

        public float SpeedOrientedFrameScalingFactor()
        {
            //#if DEBUG
            //            return 1.0f; //Juts disabled because it makes it hard to debug collisions. 
            //#endif
            float weightedThrottlePercent = (
                (_engine.Player.Sprite.MovementVector.Magnitude() / _engine.Player.Sprite.Speed) * 0.8f //80% of zoom is standard velocity
                 + (_engine.Player.Sprite.Throttle <= 1 ? 1 : _engine.Player.Sprite.Throttle / _engine.Player.Sprite.MaxThrottle) * 0.2f //20% of the zoom will be the "boost".
                ).Clamp(0, 1);

            return BaseDrawScale + ((1 - BaseDrawScale) * weightedThrottlePercent);
        }

        public float BaseDrawScale => 100.0f / _engine.Settings.OverdrawScale / 100.0f;

        /// <summary>
        /// The number of extra pixels to draw beyond the NaturalScreenSize.
        /// </summary>
        public Size OverdrawSize { get; private set; }

        /// <summary>
        /// The total size of the rendering surface (no scaling).
        /// </summary>
        public Size TotalCanvasSize { get; private set; }

        public float TotalCanvasDiagonal { get; private set; }

        public CosVector CenterCanvas;
        public CosVector CenterOfCurrentScreen => RenderWindowPosition + CenterCanvas;


        /// <summary>
        /// The size of the screen with no scaling.
        /// </summary>
        public Size NaturalScreenSize { get; private set; }

        /// <summary>
        /// The bounds of the screen with no scaling.
        /// </summary>
        public RectangleF NaturalScreenBounds =>
            new RectangleF(OverdrawSize.Width / 2.0f, OverdrawSize.Height / 2.0f, NaturalScreenSize.Width, NaturalScreenSize.Height);

        /// <summary>
        /// The total bounds of the drawing surface (canvas) natural + overdraw (with no scaling).
        /// </summary>
        public RectangleF TotalCanvasBounds => new RectangleF(0, 0, TotalCanvasSize.Width, TotalCanvasSize.Height);

        public RectangleF GetCurrentScaledScreenBounds()
        {
            var scale = SpeedOrientedFrameScalingFactor();

            if (scale < -1 || scale > 1)
            {
                throw new ArgumentException("Scale must be in the range [-1, 1].");
            }

            float centerX = TotalCanvasSize.Width * 0.5f;
            float centerY = TotalCanvasSize.Height * 0.5f;

            float smallerWidth = (float)(TotalCanvasSize.Width * scale);
            float smallerHeight = (float)(TotalCanvasSize.Height * scale);

            float left = centerX - smallerWidth * 0.5f;
            float top = centerY - smallerHeight * 0.5f;
            float right = smallerWidth;
            float bottom = smallerHeight;

            if (scale >= 0)
            {
                return new RectangleF(left, top, right, bottom);
            }
            else
            {
                //TODO: Zoom-in is untested.
                return new RectangleF(right, bottom, left, top);
            }
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
                        RenderWindowPosition.Y + CosRandom.Between(0, TotalCanvasSize.Height));
                }
                else
                {
                    return new CosVector(
                        RenderWindowPosition.X + CosRandom.Between(minOffscreenDistance, maxOffscreenDistance),
                        RenderWindowPosition.Y + CosRandom.Between(0, TotalCanvasSize.Height));
                }
            }
            else
            {
                if (CosRandom.FlipCoin())
                {
                    return new CosVector(
                        RenderWindowPosition.X + TotalCanvasSize.Width + CosRandom.Between(minOffscreenDistance, maxOffscreenDistance),
                        RenderWindowPosition.Y + CosRandom.Between(0, TotalCanvasSize.Height));
                }
                else
                {
                    return new CosVector(
                        RenderWindowPosition.X + TotalCanvasSize.Width + CosRandom.Between(minOffscreenDistance, maxOffscreenDistance),
                        RenderWindowPosition.Y + -CosRandom.Between(0, TotalCanvasSize.Height));
                }
            }
        }

        public DisplayManager(EngineCore engine, Control drawingSurface)
        {
            _engine = engine;
            DrawingSurface = drawingSurface;
            NaturalScreenSize = new Size(drawingSurface.Width, drawingSurface.Height);

            Screen = Screen.FromHandle(drawingSurface.Handle);

            int totalSizeX = (int)(NaturalScreenSize.Width * _engine.Settings.OverdrawScale);
            int totalSizeY = (int)(NaturalScreenSize.Height * _engine.Settings.OverdrawScale);

            if (totalSizeX % 2 != 0) totalSizeX++;
            if (totalSizeY % 2 != 0) totalSizeY++;

            TotalCanvasSize = new Size(totalSizeX, totalSizeY);
            OverdrawSize = new Size(totalSizeX - NaturalScreenSize.Width, totalSizeY - NaturalScreenSize.Height);
            CenterCanvas = new CosVector(TotalCanvasSize.Width / 2.0f, TotalCanvasSize.Height / 2.0f);

            TotalCanvasDiagonal = (float)Math.Sqrt(TotalCanvasSize.Width * TotalCanvasSize.Width + TotalCanvasSize.Height * TotalCanvasSize.Height);
        }

        public CosQuadrant GetQuadrant(float x, float y)
        {
            var coordinates = new Point((int)(x / NaturalScreenSize.Width), (int)(y / NaturalScreenSize.Height));

            if (Quadrants.ContainsKey(coordinates) == false)
            {
                var absoluteBounds = new Rectangle(
                    NaturalScreenSize.Width * coordinates.X,
                    NaturalScreenSize.Height * coordinates.Y,
                    NaturalScreenSize.Width,
                    NaturalScreenSize.Height);

                var quad = new CosQuadrant(coordinates, absoluteBounds);

                Quadrants.Add(coordinates, quad);
            }

            return Quadrants[coordinates];
        }
    }
}
