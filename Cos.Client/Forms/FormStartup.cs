using Cos.Client.Hardware;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Cos.Client
{
    public partial class FormStartup : Form
    {
        internal class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern bool ReleaseCapture();

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

            public const int WM_NCLBUTTONDOWN = 0xA1;
            public const int HTCAPTION = 0x2;
        }

        public Screen CurrentScreen { get; private set; }

        public FormStartup()
        {
            InitializeComponent();
            CurrentScreen = Screen.FromPoint(Cursor.Position);
            this.CenterFormOnScreen(CurrentScreen);
        }

        private void FormStartup_Load(object sender, EventArgs e)
        {
            AcceptButton = buttonStart;
            CancelButton = buttonExit;

            buttonStart.Focus();

            TopMost = false;
            if (BackgroundImage != null)
            {
                Width = BackgroundImage.Width;
                Height = BackgroundImage.Height;
            }
            StartPosition = FormStartPosition.CenterScreen;
            Opacity = 0;
            TransparencyKey = Color.FromArgb(12, 10, 12);
            BackColor = TransparencyKey;

            buttonExit.Top = Height - (buttonExit.Height + 25);
            buttonSettings.Top = Height - (buttonSettings.Height + 25);
            buttonStart.Top = Height - (buttonStart.Height + 25);

            buttonSettings.Left = (Width / 2) - (buttonSettings.Width / 2);
            buttonExit.Left = buttonSettings.Left - (buttonExit.Width + 25);
            buttonStart.Left = buttonSettings.Left + (buttonStart.Width + 25);

            MouseDown += new MouseEventHandler(Form_MouseDown);
            Shown += FormStartup_Shown;
            Move += FormStartup_Move;

            var timer = new Timer()
            {
                Enabled = true,
                Interval = 1,
            };

            timer.Tick += (object? sender, EventArgs e) =>
            {
                Opacity += 0.05;
                if (Opacity >= 1)
                {
                    timer.Stop();
                }
            };

            timer.Start();
        }

        private void FormStartup_Move(object? sender, EventArgs e)
        {
            CurrentScreen = this.GetCurrentScreen();
        }

        private void FormStartup_Shown(object? sender, EventArgs e)
        {
            try
            {
                /*
                using (var player = new SoundPlayer(@"..\..\..\Assets\Splash.wav"))
                {
                    player.Play();
                }
                */
            }
            catch { }
        }

        private void Form_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(this.Handle, NativeMethods.WM_NCLBUTTONDOWN, (IntPtr)NativeMethods.HTCAPTION, IntPtr.Zero);
            }
        }

        private void ButtonExit_Click(object? sender, EventArgs e)
        {
            CurrentScreen = this.GetCurrentScreen();
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ButtonSettings_Click(object? sender, EventArgs e)
        {
            CurrentScreen = this.GetCurrentScreen();
            using var form = new FormSettings(CurrentScreen);
            form.ShowDialog();
        }

        private void ButtonStart_Click(object? sender, EventArgs e)
        {
            CurrentScreen = this.GetCurrentScreen();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
