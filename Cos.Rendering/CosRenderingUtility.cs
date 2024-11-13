using Cos.Library;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cos.Rendering
{
    public static class CosRenderingUtility
    {
        /// <summary>
        /// Gets a random color that would be associated with fire.
        /// </summary>
        /// <returns></returns>
        static public Color4 GetRandomHotColor()
        {
            float hue = CosRandom.Between(0, 60);
            float saturation = (float)CosRandom.Between(0.8f, 1.0f);
            float lightness = (float)CosRandom.Between(0.5f, 1);
            return RGBFromHSL(hue, saturation, lightness);
        }

        /// <summary>
        /// RGB from HSL (hue, saturation, lightness).
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="saturation"></param>
        /// <param name="lightness"></param>
        static public Color4 RGBFromHSL(float hue, float saturation, float lightness)
        {
            float c = (1 - Math.Abs(2 * lightness - 1)) * saturation;
            float x = c * (1 - Math.Abs(hue / 60 % 2 - 1));
            float m = lightness - c / 2;

            float r, g, b;

            if (0 <= hue && hue < 60)
            {
                r = c; g = x; b = 0;
            }
            else if (60 <= hue && hue < 120)
            {
                r = x; g = c; b = 0;
            }
            else if (120 <= hue && hue < 180)
            {
                r = 0; g = c; b = x;
            }
            else if (180 <= hue && hue < 240)
            {
                r = 0; g = x; b = c;
            }
            else if (240 <= hue && hue < 300)
            {
                r = x; g = 0; b = c;
            }
            else
            {
                r = c; g = 0; b = x;
            }

            return new Color4(r + m, g + m, b + m, 1);
        }

        public static RawRectangleF CalculateCenterCopyRectangle(Size2F largerSize, float percentage)
        {
            if (percentage < -1 || percentage > 1)
            {
                throw new ArgumentException("Percentage must be in the range [-1, 1].");
            }

            float centerX = largerSize.Width * 0.5f;
            float centerY = largerSize.Height * 0.5f;

            float smallerWidth = largerSize.Width * percentage;
            float smallerHeight = largerSize.Height * percentage;

            float left = centerX - smallerWidth * 0.5f;
            float top = centerY - smallerHeight * 0.5f;
            float right = left + smallerWidth;
            float bottom = top + smallerHeight;

            if (percentage >= 0)
            {
                return new RawRectangleF(left, top, right, bottom);
            }
            else
            {
                return new RawRectangleF(right, bottom, left, top);
            }
        }

        public static string GetGraphicsAdaptersDescriptions()
        {
            var text = new StringBuilder();
            using (var factory = new Factory1())
            {
                foreach (var adapter in factory.Adapters)
                {
                    if (adapter.Description.Description != "Microsoft Basic Render Driver")
                    {
                        string adapterName = adapter.Description.Description;
                        var videoMemory = adapter.Description.DedicatedVideoMemory / 1024.0 / 1024.0;

                        text.AppendLine($"\"{adapterName}\" : Dedicated Video Memory {videoMemory:n2}MB");
                    }
                }
            }

            return text.ToString();
        }

        public static List<CosGraphicsAdapter> GetGraphicsAdapters()
        {
            var result = new List<CosGraphicsAdapter>();
            using (var factory = new Factory1())
            {
                foreach (var adapter in factory.Adapters)
                {
                    if (adapter.Description.Description != "Microsoft Basic Render Driver")
                    {
                        result.Add(new CosGraphicsAdapter(adapter.Description.DeviceId, adapter.Description.Description)
                        {
                            VideoMemoryMb = adapter.Description.DedicatedVideoMemory / 1024.0 / 1024.0
                        });
                    }
                }
            }

            return result;
        }

        public static float GetScreenRefreshRate(Screen screen, int deviceId)
        {
            using var factory = new Factory1();
            foreach (var adapter in factory.Adapters)
            {
                if (adapter.Description.DeviceId == deviceId)
                {
                    foreach (var output in adapter.Outputs)
                    {
                        if (output.Description.DeviceName.Equals(screen.DeviceName, StringComparison.OrdinalIgnoreCase))
                        {
                            var displayModes = output.GetDisplayModeList(Format.R8G8B8A8_UNorm, DisplayModeEnumerationFlags.Interlaced);

                            var nativeMode = displayModes.OrderByDescending(mode => mode.Width * mode.Height)
                                .ThenByDescending(o => o.RefreshRate.Numerator / o.RefreshRate.Denominator).FirstOrDefault();

                            var refreshRate = nativeMode.RefreshRate.Numerator / (float)nativeMode.RefreshRate.Denominator;

                            return refreshRate < 30f ? 30f : refreshRate;
                        }
                    }
                }
            }

            return 60; //A safe default, I would think.
        }
    }
}
