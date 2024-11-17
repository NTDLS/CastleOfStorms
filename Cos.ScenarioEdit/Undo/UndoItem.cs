using Cos.Engine.Sprite._Superclass._Root;
using Cos.Library.Mathematics;
using System.Collections.Generic;

namespace ScenarioEdit.Undo
{
    public class UndoItem
    {
        public enum ActionPerformed
        {
            Moved,
            Deleted,
            Created,
        }

        public ActionPerformed Action { get; set; }
        public List<SpriteBase> Tiles { get; set; } = new();

        /// <summary>
        /// Used to reverse tile movements.
        /// </summary>
        public CosVector? Offset { get; set; }
    }
}
