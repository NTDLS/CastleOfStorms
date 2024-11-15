using Cos.Engine;
using Cos.Engine.Sprite;
using Cos.Engine.Sprite._Superclass._Root;
using Cos.Library.Mathematics;
using Cos.ScenarioEdit.Hardware;
using NTDLS.Helpers;
using ScenarioEdit.Tiling;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static Cos.Library.CosConstants;
using static ScenarioEdit.EditorConstants;
using static ScenarioEdit.UndoItem;

namespace ScenarioEdit
{
    public partial class FormMain : Form
    {
        private Random _random = new();
        private readonly UndoBuffer _undoBuffer;
        private readonly NTDLS.Semaphore.PessimisticCriticalResource<EngineCore> _engine;
        private readonly Control _drawingSurface;
        private PrimaryMode _currentPrimaryMode = PrimaryMode.Select;
        private Point? _mouseDownPos = null;
        private CosVector? _mouseDownRenderWindowPosition;
        readonly List<SpriteBase> _hoverIntersections = new();
        private SpriteBase? _lastHoverTile;

        public FormMain()
        {
            InitializeComponent();

            _drawingSurface = new Control();
            Controls.Add(_drawingSurface);
            var engineCode = new EngineCore(_drawingSurface, CosEngineInitializationType.None);
            _engine = new NTDLS.Semaphore.PessimisticCriticalResource<EngineCore>(engineCode);
            _undoBuffer = new UndoBuffer(engineCode);
        }

        public FormMain(Screen screen)
        {
            InitializeComponent();

            treeViewTiles.ImageList = TreeNodeFactory.AssetBrowserImageList;

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

            var settings = EngineCore.LoadSettings();

            this.CenterFormOnScreen(screen, settings.Resolution);

            _drawingSurface = new Control
            {
                Dock = DockStyle.Fill
            };
            splitContainerBody.Panel1.Controls.Add(_drawingSurface);

            var engineCore = new EngineCore(_drawingSurface, CosEngineInitializationType.Edit);

            _engine = new NTDLS.Semaphore.PessimisticCriticalResource<EngineCore>(engineCore);
            _undoBuffer = new UndoBuffer(engineCore);

            engineCore.OnShutdown += (EngineCore sender) =>
            {   //If the engine is stopped, close the main form.
                Invoke((MethodInvoker)delegate
                {
                    Close();
                });
            };

            Shown += (object? sender, EventArgs e)
                => engineCore.StartEngine();

            FormClosed += (sender, e)
                => engineCore.ShutdownEngine();

            _drawingSurface.GotFocus += (object? sender, EventArgs e) => engineCore.Display.SetIsDrawingSurfaceFocused(true);
            _drawingSurface.LostFocus += (object? sender, EventArgs e) => engineCore.Display.SetIsDrawingSurfaceFocused(false);

            _drawingSurface.Select();
            _drawingSurface.Focus();

            toolStripButtonShapeMode.Click += ToolStripButtonShapeMode_Click;
            toolStripButtonSelectMode.Click += ToolStripButtonSelectMode_Click;
            toolStripButtonInsertMode.Click += ToolStripButtonInsertMode_Click;
            toolStripButtonUndo.Click += UndoToolStripMenuItem_Click;
            toolStripButtonRedo.Click += RedoToolStripMenuItem_Click;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            /*
            drawingsurface.PreviewKeyDown += drawingsurface_PreviewKeyDown;
            drawingsurface.MouseDoubleClick += new MouseEventHandler(drawingsurface_MouseDoubleClick);
            */

            _drawingSurface.MouseClick += DrawingSurface_MouseClick;
            _drawingSurface.MouseMove += DrawingSurface_MouseMove;
            _drawingSurface.MouseDown += DrawingSurface_MouseDown;
            _drawingSurface.MouseUp += DrawingSurface_MouseUp;

            //_undoBuffer = new UndoBuffer(_core);

            PopulateAllMaterials();
        }

        #region Toolbar click events.

        private void RedoToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            _undoBuffer.RollForward();
        }

        private void UndoToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            _undoBuffer.RollBack();
        }

        private void ToolStripButtonShapeMode_Click(object? sender, EventArgs e)
        {
            _currentPrimaryMode = PrimaryMode.Shape;
            toolStripButtonInsertMode.Checked = false;
            toolStripButtonSelectMode.Checked = false;
            toolStripButtonShapeMode.Checked = true;

            //ClearMultiSelection();
        }

        private void ToolStripButtonSelectMode_Click(object? sender, EventArgs e)
        {
            _currentPrimaryMode = PrimaryMode.Select;
            toolStripButtonInsertMode.Checked = false;
            toolStripButtonSelectMode.Checked = true;
            toolStripButtonShapeMode.Checked = false;

            //ClearMultiSelection();
        }

        private void ToolStripButtonInsertMode_Click(object? sender, EventArgs e)
        {
            _currentPrimaryMode = PrimaryMode.Insert;
            toolStripButtonInsertMode.Checked = true;
            toolStripButtonSelectMode.Checked = false;
            toolStripButtonShapeMode.Checked = false;

            //ClearMultiSelection();
        }

        #endregion

        private void DrawingSurface_MouseUp(object? sender, MouseEventArgs e)
        {
            //Here we keep keep track of the fact that the mouse is no longer being held down.
            _mouseDownPos = null;
            _mouseDownRenderWindowPosition = null;
        }

        private void DrawingSurface_MouseDown(object? sender, MouseEventArgs e)
        {
            _engine.Use(o =>
            {
                //Here we keep track of the position that the mouse was at when the mouse was pressed. We use this value for multiple purposes.
                _mouseDownPos = new Point(e.X, e.Y);
                _mouseDownRenderWindowPosition = o.Display.RenderWindowPosition.Clone();
            });
        }

        private void DrawingSurface_MouseMove(object? sender, MouseEventArgs e)
        {
            _engine.Use(o =>
            {
                if (e.Button == MouseButtons.Middle && _mouseDownPos != null && _mouseDownRenderWindowPosition != null)
                {
                    //Used to drag the background offset (RenderWindowPosition).
                    o.Display.RenderWindowPosition.X = _mouseDownRenderWindowPosition.X + _mouseDownPos.Value.X - e.X;
                    o.Display.RenderWindowPosition.Y = _mouseDownRenderWindowPosition.Y + _mouseDownPos.Value.Y - e.Y;
                }

                var worldX = (e.X + o.Display.RenderWindowPosition.X);
                var worldY = (e.Y + o.Display.RenderWindowPosition.Y);

                var snappedX = (worldX + 16) - ((worldX + 16) % 32);
                var snappedY = (worldY + 16) - ((worldY + 16) % 32);

                toolStripStatusLabelMouseXY.Text = $"Window: {e.X:n0}x,{e.Y:n0}, World: {worldX:n0}x,{worldY:n0}, Tile: {snappedX:n0}x,{snappedY:n0}";

                if (_lastHoverTile != null)
                {
                    string hoverText = $"[{_lastHoverTile.UID}]";
                    toolStripStatusLabelHoverObject.Text = hoverText;
                }
                else
                {
                    toolStripStatusLabelHoverObject.Text = "<none>";
                }
                //toolStripStatusLabelDebug.Text

                var intersections = o.Sprites.Intersections(worldX, worldY, 1, 1);

                foreach (var previousIntersection in _hoverIntersections)
                {
                    previousIntersection.IsHoverHighlighted = false;
                }

                _hoverIntersections.Clear();
                _hoverIntersections.AddRange(intersections.ToList());

                foreach (var previousIntersection in _hoverIntersections)
                {
                    previousIntersection.IsHoverHighlighted = true;
                }

                _lastHoverTile = _hoverIntersections.OrderByDescending(o => o.UID).FirstOrDefault();
            });
        }

        private void DrawingSurface_MouseClick(object? sender, MouseEventArgs e)
        {
            _engine.Use(o =>
            {
                var worldX = (e.X + o.Display.RenderWindowPosition.X);
                var worldY = (e.Y + o.Display.RenderWindowPosition.Y);

                //Add tile.
                if (_currentPrimaryMode == PrimaryMode.Insert && e.Button == MouseButtons.Left)
                {
                    var spriteTile = new SpriteTile(o, @"Tiles\Overworld\Dirt\Center\1.png");

                    var snappedX = (worldX + 16) - ((worldX + 16) % 32);
                    var snappedY = (worldY + 16) - ((worldY + 16) % 32);

                    spriteTile.X = snappedX;
                    spriteTile.Y = snappedY;

                    _undoBuffer.Record(spriteTile, ActionPerformed.Created);
                    o.Sprites.Add(spriteTile);
                    //spriteTile.CenterInUniverse();
                }
                //Delete tile.
                else if (_currentPrimaryMode == PrimaryMode.Insert && e.Button == MouseButtons.Right)
                {
                    var intersections = o.Sprites.Intersections(worldX, worldY, 1, 1);

                    foreach (var intersection in intersections)
                    {
                        _undoBuffer.Record(intersection, ActionPerformed.Deleted);
                        intersection.QueueForDelete();
                    }
                }
            });
        }

        void PopulateAllMaterials()
        {
            string assetsPath = @"C:\NTDLS\CastleOfStorms\Installer\Assets";

            foreach (string d in Directory.GetDirectories(assetsPath))
            {
                if (Path.GetFileName(d).StartsWith('@'))
                {
                    continue;
                }

                PopulateAssetDirectory(d);
            }
        }

        private void PopulateAssetDirectory(string directory, TreeNode? parentNode = null)
        {
            if (File.Exists(Path.Combine(directory, "tilepack.json")))
            {
                //This is a tile pack.
                PopulateTilePack(parentNode.EnsureNotNull(), directory);
            }
            else if (File.Exists(Path.Combine(directory, "metadata.json")))
            {
                //This folder contains individual tiles.
            }
            else
            {
                //This is just a folder.

                var thisFolder = TreeNodeFactory.CreateTreeNodeFolder(directory);
                (parentNode?.Nodes ?? treeViewTiles.Nodes).Add(thisFolder);

                foreach (string d in Directory.GetDirectories(directory))
                {
                    if (Path.GetFileName(d).StartsWith('@'))
                    {
                        continue;
                    }

                    PopulateAssetDirectory(d, thisFolder);
                }
            }
        }

        private void PopulateTilePack(TreeNode parentNode, string path)
        {
            var tilePackNode = TreeNodeFactory.CreateTreeNodeTilePack(path);
            parentNode.Nodes.Add(tilePackNode);
        }

        /*
        public void PopChildNodes(TreeNode parent, string partialPath)
        {

            foreach (string d in Directory.GetDirectories(Constants.BaseCommonAssetPath + _partialTilesPath + partialPath))
            {
                var directory = Path.GetFileName(d);
                if (Utility.IgnoreFileName(directory) || directory.ToLower() == "player")
                {
                    continue;
                }

                var directoryNode = parent.Nodes.Add(_partialTilesPath + directory, directory, "<folder>");
                directoryNode.Nodes.Add("<dummy>");
            }

            foreach (var f in Directory.GetFiles(Constants.BaseCommonAssetPath + _partialTilesPath + partialPath, "*.png"))
            {
                if (Utility.IgnoreFileName(f))
                {
                    continue;
                }
                var file = new FileInfo(f);

                string fileKey = $"{_partialTilesPath}{partialPath}\\{Path.GetFileNameWithoutExtension(file.Name)}";

                _assetBrowserImageList.Images.Add(fileKey, SpriteCache.GetBitmapCached(file.FullName));

                parent.Nodes.Add(fileKey, Path.GetFileNameWithoutExtension(file.Name), fileKey, fileKey);
            }
        }
        */
    }
}
