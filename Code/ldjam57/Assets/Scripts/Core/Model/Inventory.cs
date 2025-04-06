using System.Collections.Generic;

using Assets.Scripts.Core.Model.Inventories;

namespace Assets.Scripts.Core.Model
{
    public class Inventory
    {
        public List<MiningToolInventoryItem> MiningTools { get; set; } = new List<MiningToolInventoryItem>();
        public List<TransportInventoryItem> VerticalTransports { get; set; } = new List<TransportInventoryItem>();
        public List<TransportInventoryItem> HorizontalTransports { get; set; } = new List<TransportInventoryItem>();

    }
}
