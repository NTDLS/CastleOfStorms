using System;
using System.Collections.Generic;
using System.Linq;

namespace Cos.GameEngine.Manager
{
    internal static class SpriteCollectionExtensions
    {
        public static List<T> OfTypes<T>(this List<T> sprites, Type[] types)
        {
            var result = new List<T>();
            foreach (var type in types)
            {
                result.AddRange(sprites.Where(o => o != null && type.IsAssignableFrom(o.GetType())));
            }

            return result;
        }
    }
}
