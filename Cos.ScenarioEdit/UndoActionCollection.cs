using Cos.Engine.Sprite._Superclass._Root;
using Cos.Library.Mathematics;
using System.Collections.Generic;
using static ScenarioEdit.UndoItem;

namespace ScenarioEdit
{
    public class UndoActionCollection
    {
        public List<UndoItem> Actions { get; set; } = new();

        public void Record(List<SpriteBase> tiles, ActionPerformed action, CosVector? offset = null)
        {
            var item = new UndoItem()
            {
                Tiles = tiles,
                Action = action,
                Offset = offset
            };

            Actions.Add(item);
        }

        public void Record(SpriteBase tile, ActionPerformed action)
        {

            var tiles = new List<SpriteBase>
            {
                tile
            };

            Actions.Add(new UndoItem()
            {
                Tiles = tiles,
                Action = action
            });
        }
    }
}
