using System;

using GameFrame.Core.Math;

namespace Assets.Scripts.Core.Model
{
    public class MiningTool
    {
        public String Reference { get; set; }
        public String Name { get; set; }
        public Boolean IsUnlocked { get; set; }
        public Vector2 Size { get; set; }        
        public String Sprite { get; set; }
        public Double SpeedFactor { get; set; }
        public Double Capacity { get; set; }
        public Boolean IsUnlockable { get; set; }
        public Decimal UnlockCost { get; set; }
        public Decimal PurchaseCost { get; set; }
        public Decimal OperatingCost { get; set; }
        public String Sound { get; set; }
    }
}
