using Cos.Client.Hardware;
using Cos.Engine;
using Cos.Engine.Sprite._Superclass._Root;
using Cos.Engine.Sprite.Enemy._Superclass;
using Cos.Library.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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

            if (settings.FullScreen)
            {
                this.SetFullScreenOnMonitor(screen);
            }
            else
            {
                this.CenterFormOnScreen(screen, settings.Resolution);
            }

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

            if (settings.EnableSpriteInterrogation)
            {
                drawingSurface.MouseDown += FormRenderTarget_MouseDown;
                drawingSurface.MouseMove += FormRenderTarget_MouseMove;
            }
        }

        #region Debug interactions.
        private void FormRenderTarget_MouseMove(object? sender, MouseEventArgs e)
        {
            float x = e.X + _engine.Display.OverdrawSize.Width / 2;
            float y = e.Y + _engine.Display.OverdrawSize.Height / 2;

            //Debug.Print($"x{x:n1}, y{y:n1} => Player x{_engine.Player.Sprite.X:n1},x{_engine.Player.Sprite.Y:n1}");

            foreach (var sprite in highlightedSprites)
            {
                sprite.IsHighlighted = false;
            }

            highlightedSprites.Clear();

            var sprites = _engine.Sprites.RenderLocationIntersections(new CosVector(x, y), new CosVector(1, 1)).ToList();
            if (_engine.Player.Sprite.RenderLocationIntersectsAABB(new CosVector(x, y), new CosVector(1, 1)))
            {
                sprites.Add(_engine.Player.Sprite);
            }

            foreach (var sprite in sprites.Where(o => o.IsHighlighted == false))
            {
                highlightedSprites.Add(sprite);
                sprite.IsHighlighted = true;
            }
        }

        private void FormRenderTarget_MouseDown(object? sender, MouseEventArgs e)
        {
            float x = e.X + _engine.Display.OverdrawSize.Width / 2;
            float y = e.Y + _engine.Display.OverdrawSize.Height / 2;

            var sprites = _engine.Sprites.RenderLocationIntersectionsEvenInvisible(new CosVector(x, y), new CosVector(1, 1)).ToList();
            if (_engine.Player.Sprite.RenderLocationIntersectsAABB(new CosVector(x, y), new CosVector(1, 1)))
            {
                sprites.Add(_engine.Player.Sprite);
            }

            var sprite = sprites.FirstOrDefault();

            if (sprite != null)
            {
                if (e.Button == MouseButtons.Right)
                {
                    var menu = new ContextMenuStrip();

                    menu.ItemClicked += Menu_ItemClicked;
                    if (sprite is SpriteEnemyBase)
                    {
                        menu.Items.Add("Save Brain").Tag = sprite;
                        menu.Items.Add("View Brain").Tag = sprite;
                    }
                    menu.Items.Add("Delete").Tag = sprite;
                    menu.Items.Add("Watch").Tag = sprite;

                    var location = new Point((int)e.X + 10, (int)e.Y);
                    menu.Show(_engine.Display.DrawingSurface, location);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    var text = new StringBuilder();

                    text.AppendLine($"Type: {sprite.GetType().Name}");
                    text.AppendLine($"UID: {sprite.UID}");
                    text.AppendLine($"Location: {sprite.Location}");

                    if (sprite is SpriteEnemyBase enemy)
                    {
                        text.AppendLine($"Hit Points: {enemy.HullHealth:n0}");
                        text.AppendLine($"Shield Points: {enemy.ShieldHealth:n0}");
                        text.AppendLine($"Speed: {enemy.Speed:n2}");
                        text.AppendLine($"Angle: {enemy.Orientation.Degrees:n2}° {enemy.Orientation:n2}");
                        //text.AppendLine($"Throttle Percent: {enemy.Velocity.ForwardVelocity:n2}");
                    }

                    if (text.Length > 0)
                    {
                        var location = new Point((int)e.X, (int)e.Y - sprite.Size.Height);
                        _interrogationTip.Show(text.ToString(), _engine.Display.DrawingSurface, location, 5000);
                    }
                }
            }
        }

        private void Menu_ItemClicked(object? sender, ToolStripItemClickedEventArgs e)
        {
            if (sender == null) return;
            var menu = (ContextMenuStrip)sender;

            menu.Close();

            var sprite = e.ClickedItem?.Tag as SpriteBase;
            if (sprite == null) return;
        }

        #endregion

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
