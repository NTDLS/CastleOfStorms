using Cos.Library.Mathematics;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cos.GameEngine.Sprite.SupportingClasses.Metadata
{
    public class InteractiveSpriteAttachment
    {
        public InteractiveSpriteAttachment() { }

        public string Type { get; set; } = string.Empty;
        public float X { get; set; }
        public float Y { get; set; }

        [JsonIgnore]
        public CosVector LocationRelativeToOwner { get => new CosVector(X, Y); }

        public List<InteractiveSpriteWeapon> Weapons { get; set; } = new();
    }
}
