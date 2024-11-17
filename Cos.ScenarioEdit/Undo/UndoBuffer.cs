using Cos.Engine;
using Cos.Engine.Sprite._Superclass._Root;
using System;
using System.Collections.Generic;
using System.Linq;
using static ScenarioEdit.Undo.UndoItem;

namespace ScenarioEdit.Undo
{
    public class UndoBuffer(EngineCore core)
    {
        private enum Direction
        {
            Forward,
            Backward
        }

        private int _undoIndex = 0;
        private readonly EngineCore _core = core;
        private readonly List<UndoItemCollection> _undoItemCollection = new();

        /// <summary>
        /// Record a single action.
        /// </summary>
        public void Record(SpriteBase tile, ActionPerformed action)
        {
            var undoActionCollection = new UndoItemCollection();
            undoActionCollection.Record(tile, action);
            Record(undoActionCollection);
        }

        /// <summary>
        /// Record a group of actions.
        /// </summary>
        public void Record(UndoItemCollection collection)
        {
            if (_undoIndex != _undoItemCollection.Count)
            {
                //Truncate the undo collection at the current index.
                var limitedSnapshot = _undoItemCollection.Take(_undoIndex).ToList();
                _undoItemCollection.Clear();
                _undoItemCollection.AddRange(limitedSnapshot);
                _undoIndex = _undoItemCollection.Count;
            }

            _undoItemCollection.Add(collection);

            _undoIndex++;
        }

        public void RollForward()
        {
            if (_undoIndex == _undoItemCollection.Count)
            {
                return;
            }

            var actionsCollection = _undoItemCollection[_undoIndex];
            foreach (var action in actionsCollection.Items)
            {
                PerformAction(action, Direction.Forward);
            }

            _undoIndex++;
        }

        public void RollBack()
        {
            if (_undoIndex == 0)
            {
                return;
            }

            _undoIndex--;

            var actionsCollection = _undoItemCollection[_undoIndex];
            foreach (var action in actionsCollection.Items)
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
                _core.Sprites.All().Where(o => o.IsSelectedHighlighted == true)
                    .ToList().ForEach(o => o.IsSelectedHighlighted = false);

                foreach (var tile in item.Tiles)
                {
                    tile.Reset();
                    tile.IsSelectedHighlighted = true;
                    _core.Sprites.Add(tile);
                }
            }
            else if (item.Action == ActionPerformed.Moved)
            {
                _core.Sprites.All().Where(o => o.IsSelectedHighlighted == true)
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
