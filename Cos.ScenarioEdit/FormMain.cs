using Cos.Engine;
using Cos.ScenarioEdit.Hardware;
using NTDLS.Helpers;
using ScenarioEdit.Tiling;
using System;
using System.IO;
using System.Windows.Forms;
using static Cos.Library.CosConstants;

namespace ScenarioEdit
{
    public partial class FormMain : Form
    {
        private Random Rand = new Random();

        private readonly EngineCore _engine;

        public FormMain()
        {
            InitializeComponent();

            var drawingSurface = new Control();
            Controls.Add(drawingSurface);
            _engine = new EngineCore(drawingSurface, CosEngineInitializationType.None);
        }

        public FormMain(Screen screen)
        {
            InitializeComponent();

            treeViewTiles.ImageList = TreeNodeFactory.AssetBrowserImageList;

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

            var settings = EngineCore.LoadSettings();

            this.CenterFormOnScreen(screen, settings.Resolution);

            var drawingSurface = new Control
            {
                Dock = DockStyle.Fill
            };
            splitContainerBody.Panel1.Controls.Add(drawingSurface);

            _engine = new EngineCore(drawingSurface, CosEngineInitializationType.Edit);

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

            drawingSurface.GotFocus += (object? sender, EventArgs e) => _engine.Display.SetIsDrawingSurfaceFocused(true);
            drawingSurface.LostFocus += (object? sender, EventArgs e) => _engine.Display.SetIsDrawingSurfaceFocused(false);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            /*
            drawingsurface.Dock = DockStyle.Fill;
            drawingsurface.BackColor = Color.FromArgb(60, 60, 60);
            drawingsurface.PreviewKeyDown += drawingsurface_PreviewKeyDown;
            drawingsurface.Paint += new PaintEventHandler(drawingsurface_Paint);
            drawingsurface.MouseClick += new MouseEventHandler(drawingsurface_MouseClick);
            drawingsurface.MouseDoubleClick += new MouseEventHandler(drawingsurface_MouseDoubleClick);
            drawingsurface.MouseDown += new MouseEventHandler(drawingsurface_MouseDown);
            drawingsurface.MouseMove += new MouseEventHandler(drawingsurface_MouseMove);
            drawingsurface.MouseUp += new MouseEventHandler(drawingsurface_MouseUp);

            drawingsurface.Select();
            drawingsurface.Focus();
            */

            //_undoBuffer = new UndoBuffer(_core);

            PopulateMaterials();
        }

        void PopulateMaterials()
        {
            string assetsPath = @"C:\NTDLS\CastleOfStorms\Installer\Assets";

            foreach (string d in Directory.GetDirectories(assetsPath))
            {
                if (Path.GetFileName(d).StartsWith('@'))
                {
                    continue;
                }

                AddAssetDirectory(d);
            }
        }


        private void AddAssetDirectory(string directory, TreeNode? parentNode = null)
        {
            if (File.Exists(Path.Combine(directory, "tilepack.json")))
            {
                //This is a tile pack.
                AddTilePack(parentNode.EnsureNotNull(), directory);
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

                    AddAssetDirectory(d, thisFolder);
                }
            }
        }

        private void AddTilePack(TreeNode parentNode, string path)
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
