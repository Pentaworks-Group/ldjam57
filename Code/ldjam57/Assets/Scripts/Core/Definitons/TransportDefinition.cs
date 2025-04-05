using System;

using GameFrame.Core.Definitions;

namespace Assets.Scripts.Core.Definitons
{
    public class TransportDefinition : BaseDefinition
    {
        public String Name { get; set; }
        public String Sprite { get; set; }
        public Double? Speed { get; set; }
        public Double? Capacity { get; set; }
        public GameFrame.Core.Math.Vector2? Size { get; set; }
        public Boolean? IsUnlockable { get; set; }
        public Decimal? UnlockCost { get; set; }
        public Decimal? PurchaseCost { get; set; }
    }
}
