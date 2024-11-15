using Cos.Engine;
using Cos.Engine.Sprite._Superclass._Root;
using Cos.Library.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using static ScenarioEdit.UndoItem;

namespace ScenarioEdit
{
    public class UndoBuffer
    {
        private enum Direction
        {
            Forward,
            Backward
        }

        public EngineCore Core { get; set; }

        public UndoBuffer(EngineCore core)
        {
            Core = core;
        }

        private int RollingIndex { get; set; }
        private List<UndoItem> Items { get; set; } = new List<UndoItem>();

        public void Record(List<SpriteBase> tiles, ActionPerformed action, CosVector? offset = null)
        {
            if (RollingIndex != Items.Count)
            {
                Items.Clear();
                RollingIndex = 0;
            }

            var item = new UndoItem()
            {
                Tiles = tiles,
                Action = action,
                Offset = offset
            };

            Items.Add(item);

            RollingIndex++;
        }

        public void Record(SpriteBase tile, ActionPerformed action)
        {
            if (RollingIndex != Items.Count)
            {
                Items.Clear();
                RollingIndex = 0;
            }

            var tiles = new List<SpriteBase>
            {
                tile
            };

            Items.Add(new UndoItem()
            {
                Tiles = tiles,
                Action = action
            });

            RollingIndex++;
        }

        public void RollForward()
        {
            if (RollingIndex == Items.Count)
            {
                return;
            }

            PerformAction(Items[RollingIndex], Direction.Forward);

            RollingIndex++;
        }

        public void RollBack()
        {
            if (RollingIndex == 0)
            {
                return;
            }

            RollingIndex--;

            PerformAction(Items[RollingIndex], Direction.Backward);
        }

        private void PerformAction(UndoItem item, Direction direction)
        {
            if ((direction == Direction.Backward && item.Action == ActionPerformed.Created)
                || (direction == Direction.Forward && item.Action == ActionPerformed.Deleted))
            {
                item.Tiles.ForEach(o => o.QueueForDelete());
                //Core.PurgeAllDeletedTiles();
            }
            else if ((direction == Direction.Backward && item.Action == ActionPerformed.Deleted)
                || (direction == Direction.Forward && item.Action == ActionPerformed.Created))
            {
                Core.Sprites.All().Where(o => o.IsSelectedHighlighted == true)
                    .ToList().ForEach(o => o.IsSelectedHighlighted = false);

                foreach (var tile in item.Tiles)
                {
                    tile.Reset();
                    tile.IsSelectedHighlighted = true;
                    Core.Sprites.Add(tile);
                }
            }
            else if (item.Action == ActionPerformed.Moved)
            {
                Core.Sprites.All().Where(o => o.IsSelectedHighlighted == true)
                   .ToList().ForEach(o => o.IsSelectedHighlighted = false);

                foreach (var tile in item.Tiles)
                {
                    if (direction == Direction.Backward && item.Offset != null)
                    {
                        tile.X += item.Offset.X;
                        tile.Y += item.Offset.Y;
                    }
                    else if (direction == Direction.Forward && item.Offset != null)
                    {
                        tile.X -= item.Offset.X;
                        tile.Y -= item.Offset.Y;
                    }

                    tile.IsSelectedHighlighted = true;
                }
            }
            else
            {
                throw new NotFiniteNumberException();
            }
        }
    }
}
