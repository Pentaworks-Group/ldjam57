using System;

using GameFrame.Core.Definitions;

namespace Assets.Scripts.Core.Definitons
{
    public class MiningToolDefinition : BaseDefinition
    {
        public String Name { get; set; }
        public GameFrame.Core.Math.Vector2? Size { get; set; }
        public String Sprite { get; set; }
        public Double? SpeedFactor { get; set; }
        public Double? Capacity { get; set; }
        public Boolean? IsUnlockable { get; set; }
        public Boolean? IsUnlocked { get; set; }
        public Decimal? UnlockCost { get; set; }
        public Decimal? PurchaseCost { get; set; }
        public Decimal? OperatingCost { get; set; }
        public String Sound {  get; set; }
    }
}
