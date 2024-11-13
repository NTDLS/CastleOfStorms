using Cos.Client.Hardware;
using Cos.Engine;
using Cos.Engine.Sprite._Superclass._Root;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static Cos.Library.CosConstants;

namespace Cos.Client
{
    public partial class FormRenderTarget : Form
    {
        private readonly List<SpriteBase> highlightedSprites = new();
        private readonly ToolTip _interrogationTip = new();
        private readonly EngineCore _engine;
        private readonly bool _fullScreen = false;

        public FormRenderTarget()
        {
            InitializeComponent();

            var drawingSurface = new Control();
            Controls.Add(drawingSurface);
            _engine = new EngineCore(drawingSurface, CosEngineInitializationType.None);
        }

        public FormRenderTarget(Screen screen)
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

            var settings = EngineCore.LoadSettings();

            this.CenterFormOnScreen(screen, settings.Resolution);

            var drawingSurface = new Control
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(drawingSurface);

            _engine = new EngineCore(drawingSurface, CosEngineInitializationType.Play);


            _engine.OnShutdown += (EngineCore sender) =>
            {   //If the engine is stopped, close the main form.
                Invoke((MethodInvoker)delegate
                {
                    Close();
                });
            };

            Shown += (object? sender, EventArgs e)
                => _engine.StartEngine();

            FormClosed += (sender, e)
                => _engine.ShutdownEngine();

            drawingSurface.MouseEnter += (object? sender, EventArgs e) => { if (_fullScreen) { Cursor.Hide(); } };
            drawingSurface.MouseLeave += (object? sender, EventArgs e) => { if (_fullScreen) { Cursor.Show(); } };

            drawingSurface.GotFocus += (object? sender, EventArgs e) => _engine.Display.SetIsDrawingSurfaceFocused(true);
            drawingSurface.LostFocus += (object? sender, EventArgs e) => _engine.Display.SetIsDrawingSurfaceFocused(false);

            drawingSurface.KeyUp += FormRenderTarget_KeyUp;
        }

        private void FormRenderTarget_KeyUp(object? sender, KeyEventArgs e)
        {
            _engine.Input.HandleSingleKeyPress(e.KeyCode);

            if (e.KeyCode == Keys.Escape)
            {
                _engine.Pause();

                if (MessageBox.Show("Are you sure you want to quit?", "Afraid to go on?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    Close();
                }
                else
                {
                    _engine.Resume();
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs paintEventArgs)
        {
            // Prevent background painting to avoid flickering
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Prevent painting to avoid flickering.
        }
    }
}
