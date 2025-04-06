using System.Collections.Generic;

using Assets.Scripts.Core.Definitons.Inventories;

namespace Assets.Scripts.Core.Definitons
{
    public class InventoryDefinition
    {
        public List<MiningToolDefinitionInventoryItem> MiningTools { get; set; }
        public List<TransportDefinitionInventoryItem> VerticalTransports { get; set; }
        public List<TransportDefinitionInventoryItem> HorizontalTransports { get; set; }
    }
}
