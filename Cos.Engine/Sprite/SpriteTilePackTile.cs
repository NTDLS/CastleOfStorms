using Cos.Engine.Sprite._Superclass._Root;
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

        public SpriteTilePackTile(EngineCore engine, string imagePath, Guid collectionId, TilePackTileType tilePackTileType)
            : base(engine)
        {
            CollectionId = collectionId;
            TilePackTileType = tilePackTileType;
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
