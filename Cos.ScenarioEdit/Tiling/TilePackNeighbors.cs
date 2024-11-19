using Cos.Engine.Sprite;
using System.Collections.Generic;

namespace ScenarioEdit.Tiling
{
    internal class TilePackNeighbors
    {
        private SpriteTilePackTile? _topLeft;
        private SpriteTilePackTile? _top;
        private SpriteTilePackTile? _topRight;
        private SpriteTilePackTile? _right;
        private SpriteTilePackTile? _bottomRight;
        private SpriteTilePackTile? _bottom;
        private SpriteTilePackTile? _bottomLeft;
        private SpriteTilePackTile? _left;

        public HashSet<SpriteTilePackTile> All { get; set; } = new();

        public SpriteTilePackTile? TopLeft
        {
            get => _topLeft;
            set
            {
                if (value != null)
                {
                    All.Add(value);
                    _topLeft = value;
                }
            }
        }
        public SpriteTilePackTile? Top
        {
            get => _top; set
            {
                if (value != null)
                {
                    All.Add(value);
                    _top = value;
                }
            }
        }
        public SpriteTilePackTile? TopRight
        {
            get => _topRight; set
            {
                if (value != null)
                {
                    All.Add(value);
                    _topRight = value;
                }
            }
        }
        public SpriteTilePackTile? Right
        {
            get => _right; set
            {
                if (value != null)
                {
                    All.Add(value);
                    _right = value;
                }
            }
        }
        public SpriteTilePackTile? BottomRight
        {
            get => _bottomRight; set
            {
                if (value != null)
                {
                    All.Add(value);
                    _bottomRight = value;
                }
            }
        }
        public SpriteTilePackTile? Bottom
        {
            get => _bottom; set
            {
                if (value != null)
                {
                    All.Add(value);
                    _bottom = value;
                }
            }
        }
        public SpriteTilePackTile? BottomLeft
        {
            get => _bottomLeft; set
            {
                if (value != null)
                {
                    All.Add(value);
                    _bottomLeft = value;
                }
            }
        }
        public SpriteTilePackTile? Left
        {
            get => _left; set
            {
                if (value != null)
                {
                    All.Add(value);
                    _left = value;
                }
            }
        }
    }
}
