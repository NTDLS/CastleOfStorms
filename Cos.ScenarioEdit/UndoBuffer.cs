using Cos.Engine;
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
        private readonly List<UndoActionCollection> _actionsCollection = new();

        public void Record(UndoActionCollection collection)
        {
            if (RollingIndex != _actionsCollection.Count)
            {
                //Truncate the undo collection at the current index.
                var limitedSnapshot = _actionsCollection.Take(RollingIndex).ToList();
                _actionsCollection.Clear();
                _actionsCollection.AddRange(limitedSnapshot);
                RollingIndex = _actionsCollection.Count;
            }

            _actionsCollection.Add(collection);

            RollingIndex++;
        }

        public void RollForward()
        {
            if (RollingIndex == _actionsCollection.Count)
            {
                return;
            }

            var actionsCollection = _actionsCollection[RollingIndex];
            foreach (var action in actionsCollection.Actions)
            {
                PerformAction(action, Direction.Forward);
            }

            RollingIndex++;
        }

        public void RollBack()
        {
            if (RollingIndex == 0)
            {
                return;
            }

            RollingIndex--;

            var actionsCollection = _actionsCollection[RollingIndex];
            foreach (var action in actionsCollection.Actions)
            {
                PerformAction(action, Direction.Backward);
            }
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
