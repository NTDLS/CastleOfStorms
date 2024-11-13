using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Cos.Client.Hardware
{
    #region DEVMODE struct.

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DEVMODE
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmDeviceName;
        public ushort dmSpecVersion;
        public ushort dmDriverVersion;
        public ushort dmSize;
        public ushort dmDriverExtra;
        public uint dmFields;
        public int dmPositionX;
        public int dmPositionY;
        public uint dmDisplayOrientation;
        public uint dmDisplayFixedOutput;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;
        public ushort dmLogPixels;
        public uint dmBitsPerPel;
        public uint dmPelsWidth;
        public uint dmPelsHeight;
        public uint dmDisplayFlags;
        public uint dmDisplayFrequency;
        public uint dmICMMethod;
        public uint dmICMIntent;
        public uint dmMediaType;
        public uint dmDitherType;
        public uint dmReserved1;
        public uint dmReserved2;
        public uint dmPanningWidth;
        public uint dmPanningHeight;
    }


    #endregion

    internal static class Display
    {
        private const int ENUM_CURRENT_SETTINGS = -1;

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        private static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

        public static uint GetControlMonitorRefreshRate(Control control)
        {
            var screen = Screen.FromControl(control);
            if (screen != null)
            {
                var deviceMode = new DEVMODE();
                if (EnumDisplaySettings(screen.DeviceName, ENUM_CURRENT_SETTINGS, ref deviceMode))
                {
                    return deviceMode.dmDisplayFrequency;
                }
            }

            return 60;
        }

        public static void CenterFormOnScreen(this Form form, Screen targetScreen, Size? size = null)
        {
            if (size != null)
            {
                form.ClientSize = (Size)size;
            }

            // Get the bounds of the target screen
            var screenBounds = targetScreen.Bounds;

            // Calculate the new position of the form (centered)
            int x = screenBounds.Left + (screenBounds.Width - form.Width) / 2;
            int y = screenBounds.Top + (screenBounds.Height - form.Height) / 2;

            // Set the new location without changing the size
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new System.Drawing.Point(x, y);
        }

        public static void SetFullScreenOnMonitor(this Form form, Screen targetMonitor)
        {
            form.FormBorderStyle = FormBorderStyle.None;
            form.WindowState = FormWindowState.Normal; // Ensure we can manually set size
            form.StartPosition = FormStartPosition.Manual;
            form.ShowInTaskbar = true;
#if !DEBUG
                form.TopMost = true; //This is a total pain for debugging.
#endif
            // Set the form's location and size to match the target monitor
            form.Bounds = targetMonitor.Bounds;

            // Maximize the form (but ensure no borders)
            form.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// Used to determine which screen the control is on.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Screen GetCurrentScreen(this Control control)
        {
            return Screen.FromControl(control);
        }
    }
}
