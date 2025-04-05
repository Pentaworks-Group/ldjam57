using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Core.Definitons
{
    public class InventoryDefinition
    {
        public List<MiningTool> Tools { get; set; }
        public List<Transport> VerticalTransports { get; set; }
        public List<Transport> HorizontalTransports { get; set; }
    }
}
