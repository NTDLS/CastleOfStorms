using System.Windows.Forms;

namespace ScenarioEdit.Tiling.TreeNodes
{
    public class TreeNodeTilePack : TreeNode
    {
        public TilePack Meta { get; private set; }

        public TreeNodeTilePack(TilePack meta, string imageKey)
        {
            Text = meta.Name;
            Meta = meta;
            ImageKey = imageKey;
            SelectedImageKey = imageKey;
        }
    }
}
