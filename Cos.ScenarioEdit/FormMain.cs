using ScenarioEdit.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ScenarioEdit
{
    public partial class FormMain : Form
    {
        private Random Rand = new Random();

        /// <summary>"
        /// The action that will be performed when clicking the left mouse button.
        /// </summary>
        public enum PrimaryMode
        {
            Insert,
            Select,
            Shape
        }

        public enum ShapeFillMode
        {
            Insert,
            Delete,
            Select,
        }

        //public UndoBuffer _undoBuffer { get; set; }
        private bool _firstShown = true;
        private bool _hasBeenModified = false;
        private string _currentMapFilename = string.Empty;
        private int _newFilenameIncrement = 1;
        private ToolTip _interrogationTip = new ToolTip();
        private Rectangle? _shapeSelectionRect = null;
        private ImageList _assetBrowserImageList = new ImageList();
        private Point _lastMouseLocation = new Point();
        private string _partialTilesPath = "Tiles\\";


        //This really shouldn't be necessary! :(
        protected override CreateParams CreateParams
        {
            get
            {
                //Paints all descendants of a window in bottom-to-top painting order using double-buffering.
                // For more information, see Remarks. This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. 
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000; //WS_EX_COMPOSITED       
                return handleParam;
            }
        }

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //splitContainerBody.Panel1.Controls.Add(drawingsurface);

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

        private void TreeViewTiles_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "<dummy>")
            {
                e.Node.Nodes.Clear();
                PopChildNodes(e.Node, e.Node.FullPath);
            }
        }

        void PopulateMaterials()
        {
            /*
            _assetBrowserImageList.Images.Add("<folder>", Resources.AssetTreeView_Folder);

            treeViewTiles.ImageList = _assetBrowserImageList;

            foreach (string d in Directory.GetDirectories(Constants.BaseCommonAssetPath + _partialTilesPath))
            {
                if (Utility.IgnoreFileName(d))
                {
                    continue;
                }
                var directory = Path.GetFileName(d);

                var directoryNode = treeViewTiles.Nodes.Add(_partialTilesPath + directory, directory, "<folder>");
                directoryNode.Nodes.Add("<dummy>");
            }
            */
        }

        public void PopChildNodes(TreeNode parent, string partialPath)
        {
            /*
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
            */
        }
    }
}
