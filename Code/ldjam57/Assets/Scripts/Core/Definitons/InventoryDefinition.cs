using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Core.Definitons
{
    public class InventoryDefinition
    {
        public List<MiningToolDefinition> Tools { get; set; }
        public List<TransportDefinition> VerticalTransports { get; set; }
        public List<TransportDefinition> HorizontalTransports { get; set; }
    }
}
