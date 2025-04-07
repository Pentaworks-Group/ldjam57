using System;

namespace Assets.Scripts.Core.Model.Inventories
{
    public class TransportInventoryItem : InventoryItem
    {
        public Transport Transport { get; set; }
        public Boolean IsVertical { get; set; }

        public override System.String GetKey()
        {
            return Transport.Reference;
        }

        public override String GetName()
        {
            return Transport.Name;
        }

        public override String GetSprite()
        {
            return Transport.Sprite;
        }
    }
}
