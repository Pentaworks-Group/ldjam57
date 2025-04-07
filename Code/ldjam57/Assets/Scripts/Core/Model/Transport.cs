using System;

namespace Assets.Scripts.Core.Model
{
    public class Transport
    {
        public String Reference { get; set; }
        public String Name { get; set; }
        public String Sprite { get; set; }
        public Double Speed { get; set; }
        public Double Capacity { get; set; }
        public GameFrame.Core.Math.Vector2 Size { get; set; }
        public Boolean IsUnlocked { get; set; }
        public Boolean IsUnlockable { get; set; }
        public Decimal UnlockCost { get; set; }
        public Decimal PurchaseCost { get; set; }
        public Decimal OperatingCost { get; set; }
        public String Sound { get; set; }

    }
}
