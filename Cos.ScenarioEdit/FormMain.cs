using Cos.Engine;
using Cos.Engine.Sprite;
using Cos.Engine.Sprite._Superclass._Root;
using Cos.Library.Mathematics;
using Cos.ScenarioEdit.Hardware;
using Microsoft.VisualBasic;
using NTDLS.Helpers;
using ScenarioEdit.Tiling;
using ScenarioEdit.Tiling.TreeNodes;
using ScenarioEdit.Undo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static Cos.Library.CosConstants;
using static ScenarioEdit.EditorConstants;
using static ScenarioEdit.Undo.UndoItem;

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

            ToolStripButtonInsertMode_Click(null, new());
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
                    string hoverText = $"[{_lastHoverTile.ToString()}]";
                    toolStripStatusLabelHoverObject.Text = hoverText;
                }
                else
                {
                    toolStripStatusLabelHoverObject.Text = "<none>";
                }

                #region De-Highlight and highlight hover tile(s).

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

                #endregion

                _lastHoverTile = _hoverIntersections.OrderByDescending(o => o.UID).FirstOrDefault();

                if (_lastHoverTile == null)
                {
                    toolStripStatusLabelDebug.Text = string.Empty;
                }
                else
                {
                    toolStripStatusLabelDebug.Text = $"{_lastHoverTile.X},{_lastHoverTile.Y}:{_lastHoverTile.X + _lastHoverTile.Size.Width},{_lastHoverTile.Y + _lastHoverTile.Size.Height}";
                }
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
                    var snappedX = (worldX + 16) - ((worldX + 16) % 32);
                    var snappedY = (worldY + 16) - ((worldY + 16) % 32);

                    PlaceTile(snappedX, snappedY);
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

        /// <summary>
        /// Places the selected which is selected in the materials view, at the specified location.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void PlaceTile(float x, float y)
        {
            //x = 256; //Debug
            //y = 256; //Debug

            _engine.Use(o =>
            {
                if (treeViewTiles.SelectedNode == null)
                {
                    return;
                }

                if (treeViewTiles.SelectedNode is TreeNodeTilePack tilePack)
                {
                    var undoItemCollection = new UndoItemCollection();

                    int randomIndex = _random.Next(tilePack.Meta.Center.Count);
                    string randomItem = tilePack.Meta.Center[randomIndex];

                    //If we are placing a tile on top of another TilePack tile from the same collection, then delete it.
                    var intersections = o.Sprites.Intersections(x, y, 1, 1)
                        .Where(o => o is SpriteTilePackTile tilePackTile && tilePackTile.CollectionId == tilePack.Meta.CollectionId);
                    foreach (var intersection in _hoverIntersections)
                    {
                        undoItemCollection.Record(intersection, ActionPerformed.Deleted);
                        intersection.QueueForDelete();
                    }

                    var spriteTile = new SpriteTilePackTile(o, new CosVector(x, y), Path.Join(tilePack.FullPath, randomItem), tilePack.Meta.CollectionId, TilePackTileType.Center)
                    {
                        TilePackTileType = TilePackTileType.Center,
                    };

                    undoItemCollection.Record(spriteTile, ActionPerformed.Created);
                    o.Sprites.Add(spriteTile);

                    EncaseTilePackTile(tilePack, spriteTile, undoItemCollection);

                    _undoBuffer.Record(undoItemCollection);
                }
                else
                {
                    //...individual tiles?
                }
            });
        }

        /*
        private List<SpriteTilePackTile> GetTileNeighbors(TreeNodeTilePack tilePack, SpriteTilePackTile tile, UndoItemCollection undoItemCollection)
        {
            var neighbors = new List<SpriteTilePackTile>();

            _engine.Use(o =>
            {
                List<CosVector> offsets =
                [
                    new CosVector(-1, -1), //top-left
                    new CosVector(0, -1), //top
                    new CosVector(+33, -1), //top-right
                    new CosVector(+33, 0), //right
                    new CosVector(+33, +33), //bottom-right
                    new CosVector(0, +33), //bottom
                    new CosVector(0, +33), //bottom-left
                    new CosVector(-1, +33), //left
                ];

                foreach (var offset in offsets)
                {
                    var collision = o.Sprites.Intersections(tile.X + offset.X, tile.Y + offset.Y, 1, 1)
                        .Where(o => o is SpriteTilePackTile tilePackTile && tilePackTile.CollectionId == tilePack.Meta.CollectionId).FirstOrDefault();
                    if (collision is SpriteTilePackTile collisionTile)
                    {
                        neighbors.Add(collisionTile);
                    }
                }
            });

            return neighbors;
        }
        */

        /// <summary>
        /// Returns information about the tile neighbors for the given tile pack tile.
        /// </summary>
        private TilePackNeighbors GetTilePackTileNeighbors(TreeNodeTilePack tilePack, SpriteTilePackTile tile)
        {
            var neighbors = new TilePackNeighbors();

            _engine.Use(o =>
            {
                #region Get tile neighbors.

                neighbors.TopLeft = o.Sprites.Intersections(tile.Location + NeighborOffsets.TopLeft, CosVector.One)
                    .Where(o => o is SpriteTilePackTile tilePackTile && tilePackTile.CollectionId == tilePack.Meta.CollectionId)
                    .FirstOrDefault() as SpriteTilePackTile;

                neighbors.Top = o.Sprites.Intersections(tile.Location + NeighborOffsets.Top, CosVector.One)
                    .Where(o => o is SpriteTilePackTile tilePackTile && tilePackTile.CollectionId == tilePack.Meta.CollectionId)
                    .FirstOrDefault() as SpriteTilePackTile;

                neighbors.TopRight = o.Sprites.Intersections(tile.Location + NeighborOffsets.TopRight, CosVector.One)
                    .Where(o => o is SpriteTilePackTile tilePackTile && tilePackTile.CollectionId == tilePack.Meta.CollectionId)
                    .FirstOrDefault() as SpriteTilePackTile;

                neighbors.Right = o.Sprites.Intersections(tile.Location + NeighborOffsets.Right, CosVector.One)
                    .Where(o => o is SpriteTilePackTile tilePackTile && tilePackTile.CollectionId == tilePack.Meta.CollectionId)
                    .FirstOrDefault() as SpriteTilePackTile;

                neighbors.BottomRight = o.Sprites.Intersections(tile.Location + NeighborOffsets.BottomRight, CosVector.One)
                    .Where(o => o is SpriteTilePackTile tilePackTile && tilePackTile.CollectionId == tilePack.Meta.CollectionId)
                    .FirstOrDefault() as SpriteTilePackTile;

                neighbors.Bottom = o.Sprites.Intersections(tile.Location + NeighborOffsets.Bottom, CosVector.One)
                    .Where(o => o is SpriteTilePackTile tilePackTile && tilePackTile.CollectionId == tilePack.Meta.CollectionId)
                    .FirstOrDefault() as SpriteTilePackTile;

                neighbors.BottomLeft = o.Sprites.Intersections(tile.Location + NeighborOffsets.BottomLeft, CosVector.One)
                    .Where(o => o is SpriteTilePackTile tilePackTile && tilePackTile.CollectionId == tilePack.Meta.CollectionId)
                    .FirstOrDefault() as SpriteTilePackTile;

                neighbors.Left = o.Sprites.Intersections(tile.Location + NeighborOffsets.Left, CosVector.One)
                    .Where(o => o is SpriteTilePackTile tilePackTile && tilePackTile.CollectionId == tilePack.Meta.CollectionId)
                    .FirstOrDefault() as SpriteTilePackTile;

                #endregion
            });

            return neighbors;
        }

        //Find all neighboring tiles that belong to this tile pack, and where an edge is found that is not a center, put the correct edge tiles on it.
        //If it is a center tile, then find its neighbors and do the same.

        /// <summary>
        /// This function is used to remove all edge neighbors and replace them where they are missing.
        /// </summary>
        private void EncaseTilePackTile(TreeNodeTilePack tilePack, SpriteTilePackTile tile, UndoItemCollection undoItemCollection)
        {
            _engine.Use(o =>
            {
                var neighbors = GetTilePackTileNeighbors(tilePack, tile);

                //Delete all edge neighbors.
                foreach (var neighbor in neighbors.All.Where(o => o.TilePackTileType != TilePackTileType.Center))
                {
                    undoItemCollection.Record(neighbor, ActionPerformed.Deleted);
                    neighbor.QueueForDelete();
                }

                #region Add tile edges.

                //Right
                if (neighbors.Right?.TilePackTileType != TilePackTileType.Center)
                {
                    int randomIndex = _random.Next(tilePack.Meta.Right.Count);
                    string randomItem = tilePack.Meta.Right[randomIndex];

                    var spriteTile = new SpriteTilePackTile(o, tile.Location + NeighborOffsets.Right, Path.Join(tilePack.FullPath, randomItem), tilePack.Meta.CollectionId, TilePackTileType.Center)
                    {
                        TilePackTileType = TilePackTileType.Right
                    };
                    o.Sprites.Add(spriteTile);
                    undoItemCollection.Record(spriteTile, ActionPerformed.Created);
                }

                //TopRight
                if (neighbors.TopRight?.TilePackTileType != TilePackTileType.Center)
                {
                    int randomIndex = _random.Next(tilePack.Meta.TopRight.Count);
                    string randomItem = tilePack.Meta.TopRight[randomIndex];

                    var spriteTile = new SpriteTilePackTile(o, tile.Location + NeighborOffsets.TopRight, Path.Join(tilePack.FullPath, randomItem), tilePack.Meta.CollectionId, TilePackTileType.Center)
                    {
                        TilePackTileType = TilePackTileType.TopRight
                    };
                    o.Sprites.Add(spriteTile);
                    undoItemCollection.Record(spriteTile, ActionPerformed.Created);
                }

                //BottomRight
                if (neighbors.BottomRight?.TilePackTileType != TilePackTileType.Center)
                {
                    int randomIndex = _random.Next(tilePack.Meta.BottomRight.Count);
                    string randomItem = tilePack.Meta.BottomRight[randomIndex];

                    var spriteTile = new SpriteTilePackTile(o, tile.Location + NeighborOffsets.BottomRight, Path.Join(tilePack.FullPath, randomItem), tilePack.Meta.CollectionId, TilePackTileType.Center)
                    {
                        TilePackTileType = TilePackTileType.BottomRight
                    };
                    o.Sprites.Add(spriteTile);
                    undoItemCollection.Record(spriteTile, ActionPerformed.Created);
                }

                //Left
                if (neighbors.Left?.TilePackTileType != TilePackTileType.Center)
                {
                    int randomIndex = _random.Next(tilePack.Meta.Left.Count);
                    string randomItem = tilePack.Meta.Left[randomIndex];

                    var spriteTile = new SpriteTilePackTile(o, tile.Location + NeighborOffsets.Left, Path.Join(tilePack.FullPath, randomItem), tilePack.Meta.CollectionId, TilePackTileType.Center)
                    {
                        TilePackTileType = TilePackTileType.Left
                    };
                    o.Sprites.Add(spriteTile);
                    undoItemCollection.Record(spriteTile, ActionPerformed.Created);
                }

                //TopLeft
                if (neighbors.TopLeft?.TilePackTileType != TilePackTileType.Center)
                {
                    int randomIndex = _random.Next(tilePack.Meta.TopLeft.Count);
                    string randomItem = tilePack.Meta.TopLeft[randomIndex];

                    var spriteTile = new SpriteTilePackTile(o, tile.Location + NeighborOffsets.TopLeft, Path.Join(tilePack.FullPath, randomItem), tilePack.Meta.CollectionId, TilePackTileType.Center)
                    {
                        TilePackTileType = TilePackTileType.TopLeft
                    };
                    o.Sprites.Add(spriteTile);
                    undoItemCollection.Record(spriteTile, ActionPerformed.Created);
                }

                //BottomLeft
                if (neighbors.BottomLeft?.TilePackTileType != TilePackTileType.Center)
                {
                    int randomIndex = _random.Next(tilePack.Meta.BottomLeft.Count);
                    string randomItem = tilePack.Meta.BottomLeft[randomIndex];

                    var spriteTile = new SpriteTilePackTile(o, tile.Location + NeighborOffsets.BottomLeft, Path.Join(tilePack.FullPath, randomItem), tilePack.Meta.CollectionId, TilePackTileType.Center)
                    {
                        TilePackTileType = TilePackTileType.BottomLeft
                    };
                    o.Sprites.Add(spriteTile);
                    undoItemCollection.Record(spriteTile, ActionPerformed.Created);
                }

                //Top
                if (neighbors.Top?.TilePackTileType != TilePackTileType.Center)
                {
                    int randomIndex = _random.Next(tilePack.Meta.Top.Count);
                    string randomItem = tilePack.Meta.Top[randomIndex];

                    var spriteTile = new SpriteTilePackTile(o, tile.Location + NeighborOffsets.Top, Path.Join(tilePack.FullPath, randomItem), tilePack.Meta.CollectionId, TilePackTileType.Center)
                    {
                        TilePackTileType = TilePackTileType.Top
                    };
                    o.Sprites.Add(spriteTile);
                    undoItemCollection.Record(spriteTile, ActionPerformed.Created);
                }

                //Bottom
                if (neighbors.Bottom?.TilePackTileType != TilePackTileType.Center)
                {
                    int randomIndex = _random.Next(tilePack.Meta.Bottom.Count);
                    string randomItem = tilePack.Meta.Bottom[randomIndex];

                    var spriteTile = new SpriteTilePackTile(o, tile.Location + NeighborOffsets.Bottom, Path.Join(tilePack.FullPath, randomItem), tilePack.Meta.CollectionId, TilePackTileType.Center)
                    {
                        TilePackTileType = TilePackTileType.Bottom
                    };
                    o.Sprites.Add(spriteTile);
                    undoItemCollection.Record(spriteTile, ActionPerformed.Created);
                }

                #endregion
            });
        }

        #region Populate matrials/assets.

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

            //Expand all top level nodes.
            foreach (TreeNode node in treeViewTiles.Nodes)
            {
                node.Expand();
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

        #endregion
    }
}
