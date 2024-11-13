using System;
using System.Collections.Generic;

namespace ScenarioEdit.Tiling
{
    /// <summary>
    /// Tile packs do not show as individual tiles in the editor, but show as the "thumbnail".
    /// Tile packs are intended to be drawn for the user automatically (center, edges, etc).
    /// These are identified bt folders that contain a file called "tilepack.json".
    /// The individual tiles for the pack should be under subfolders, each of which can contain a file called "metadata.json"
    ///     which applies to all files in the folder as well as can contain metadata files with the name of the "{tile}.json"
    ///     which overrides any metadata specified by the "metadata.json" file.
    /// </summary>
    public class TilePack
    {
        /// <summary>
        /// The guid that identifies the tile pack that the tiles are associated with,
        /// so we know which files to use when writing new tiles on an existing swatch.
        /// </summary>
        public Guid CollectionId { get; set; }

        /// <summary>
        /// The friendly name for the tile pack, to be shown in the editor.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// The image to show in the editor tile tree.
        /// </summary>
        public string Thumbnail { get; set; } = string.Empty;

        public List<string> Top { get; set; } = new();
        public List<string> Bottom { get; set; } = new();
        public List<string> Left { get; set; } = new();
        public List<string> Right { get; set; } = new();
        public List<string> Center { get; set; } = new();
        public List<string> TopRight { get; set; } = new();
        public List<string> TopLeft { get; set; } = new();
        public List<string> BottomRight { get; set; } = new();
        public List<string> BottomLeft { get; set; } = new();
    }
}
