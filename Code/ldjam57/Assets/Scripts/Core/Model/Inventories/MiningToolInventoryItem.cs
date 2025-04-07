using System;

namespace Assets.Scripts.Core.Model.Inventories
{
    public class MiningToolInventoryItem : InventoryItem
    {
        public MiningTool MiningTool { get; set; }

        public override System.String GetKey()
        {
            return MiningTool.Reference;
        }

        public override System.String GetName()
        {
            return MiningTool.Name;
        }

        public override String GetSprite()
        {
            return MiningTool.Sprite;
        }
    }
}
