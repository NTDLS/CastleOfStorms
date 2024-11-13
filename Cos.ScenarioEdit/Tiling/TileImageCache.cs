using System.Collections.Generic;
using System.Drawing;

namespace ScenarioEdit.Tiling
{
    public static class TileImageCache
    {
        private static readonly Dictionary<string, Bitmap> _bitmapCache = new();

        public static Bitmap GetBitmapCached(string path)
        {
            Bitmap? result = null;

            path = path.ToLower();

            lock (_bitmapCache)
            {
                if (_bitmapCache.ContainsKey(path))
                {
                    result = (Bitmap)_bitmapCache[path].Clone();
                }
                else
                {
                    using (var image = Image.FromFile(path))
                    using (var newBitmap = new Bitmap(image))
                    {
                        result = (Bitmap)newBitmap.Clone();
                        _bitmapCache.Add(path, result);
                    }
                }
            }

            return result;
        }
    }
}
