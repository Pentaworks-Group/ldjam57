using System;

namespace Assets.Scripts.Core.Model
{
    public abstract class InventoryItem
    {
        public Int32 Amount { get; set; }

        public abstract String GetKey();
        public abstract String GetName();
        public abstract String GetSprite();
    }
}
