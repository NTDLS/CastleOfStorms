using Cos.Engine.Sprite._Superclass._Root;
using Cos.Library.Mathematics;
using System.Collections.Generic;
using static ScenarioEdit.Undo.UndoItem;

namespace ScenarioEdit.Undo
{
    public class UndoItemCollection
    {
        public List<UndoItem> Items { get; set; } = new();

        public void Record(List<SpriteBase> tiles, ActionPerformed action, CosVector? offset = null)
        {
            var item = new UndoItem()
            {
                Tiles = tiles,
                Action = action,
                Offset = offset
            };

            Items.Add(item);
        }

        public void Record(SpriteBase tile, ActionPerformed action)
        {
            Items.Add(new UndoItem()
            {
                Tiles = [tile],
                Action = action
            });
        }
    }
}
