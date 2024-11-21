using Cos.Engine.Sprite._Superclass._Root;
using Cos.Library.Mathematics;
using System;
using static Cos.Library.CosConstants;

namespace Cos.Engine.Sprite
{
    /// <summary>
    /// These are just minimal non-collidable, non interactive, generic bitmap sprites.
    /// </summary>
    public class SpriteTilePackTile : SpriteBase
    {
        public Guid CollectionId { get; set; }

        public TilePackTileType TilePackTileType { get; set; }

        public SpriteTilePackTile(EngineCore engine, CosVector centerTileLocation, TilePackTileType tilePackTileType, string imagePath, Guid collectionId)
            : base(engine)
        {
            CollectionId = collectionId;
            TilePackTileType = tilePackTileType;

            switch (tilePackTileType)
            {
                case TilePackTileType.Top:
                    centerTileLocation += NeighborOffsets.Top;
                    break;
                case TilePackTileType.TopLeft:
                    centerTileLocation += NeighborOffsets.TopLeft;
                    break;
                case TilePackTileType.TopRight:
                    centerTileLocation += NeighborOffsets.TopRight;
                    break;
                case TilePackTileType.Right:
                    centerTileLocation += NeighborOffsets.Right;
                    break;
                case TilePackTileType.BottomRight:
                    centerTileLocation += NeighborOffsets.BottomRight;
                    break;
                case TilePackTileType.Bottom:
                    centerTileLocation += NeighborOffsets.Bottom;
                    break;
                case TilePackTileType.BottomLeft:
                    centerTileLocation += NeighborOffsets.BottomLeft;
                    break;
                case TilePackTileType.Left:
                    centerTileLocation += NeighborOffsets.Left;
                    break;
            }

            X = centerTileLocation.X;
            Y = centerTileLocation.Y;

            SetImage(imagePath);
        }

        public SpriteTilePackTile(EngineCore engine, string imagePath)
            : base(engine)
        {
            SetImage(imagePath);
        }

        public SpriteTilePackTile(EngineCore engine, SharpDX.Direct2D1.Bitmap bitmap)
            : base(engine)
        {
            SetImage(bitmap);
        }

        public SpriteTilePackTile(EngineCore engine)
            : base(engine)
        {
        }
    }
}
