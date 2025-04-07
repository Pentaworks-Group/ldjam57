using System;

namespace Assets.Scripts.Core.Definitons.Inventories
{
    public class MiningToolDefinitionInventoryItem
    {
        public Int32? Amount { get; set; }
        public String Sprite { get; set; }
        public MiningToolDefinition MiningTool { get; set; }
    }
}
