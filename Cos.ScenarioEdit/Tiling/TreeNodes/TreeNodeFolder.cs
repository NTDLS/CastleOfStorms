using System.IO;
using System.Windows.Forms;

namespace ScenarioEdit.Tiling.TreeNodes
{
    public class TreeNodeFolder : TreeNode
    {
        public string Directory { get; private set; }
        public TreeNodeFolder(string path)
        {
            Directory = path;
            Text = Path.GetFileName(path);
            ImageKey = "<folder>";
            SelectedImageKey = "<folder>";
        }
    }
}
