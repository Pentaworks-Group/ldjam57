using System;
using System.Collections.Generic;

using GameFrame.Core.Definitions;

namespace Assets.Scripts.Core.Definitons
{
    public class GameMode : BaseDefinition
    {
        public String Name { get; set; }
        public WorldDefinition World { get; set; }
        public BankDefinition Bank { get; set; }
        public Market Market { get; set; }
        public List<MiningTool> AvailableTools { get; set; }
        public InventoryDefinition Inventory { get; set; }
    }
}
