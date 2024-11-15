using NTDLS.Helpers;
using ScenarioEdit.Properties;
using ScenarioEdit.Tiling.TreeNodes;
using System.IO;
using System.Windows.Forms;

namespace ScenarioEdit.Tiling
{
    public static class TreeNodeFactory
    {
        public static ImageList AssetBrowserImageList = new ImageList();

        public static TreeNodeFolder CreateTreeNodeFolder(string name)
        {
            if (AssetBrowserImageList.Images.ContainsKey("<folder>") == false)
            {
                AssetBrowserImageList.Images.Add("<folder>", Resources.AssetTreeView_Folder);
            }

            return new TreeNodeFolder(name);
        }

        public static TreeNodeTilePack CreateTreeNodeTilePack(string path)
        {
            string imageKey = path.ToLowerInvariant();

            var tilePackMetaJson = File.ReadAllText(Path.Combine(path, "tilepack.json"));
            var tilePackMeta = Newtonsoft.Json.JsonConvert.DeserializeObject<TilePack>(tilePackMetaJson).EnsureNotNull();

            if (AssetBrowserImageList.Images.ContainsKey(imageKey) == false)
            {
                AssetBrowserImageList.Images.Add(imageKey, TileImageCache.GetBitmapCached(Path.Combine(path, tilePackMeta.Thumbnail)));
            }

            return new TreeNodeTilePack(tilePackMeta, imageKey);
        }
    }
}

