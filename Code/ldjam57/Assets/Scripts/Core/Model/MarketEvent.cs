using System;
using System.Collections.Generic;

namespace Assets.Scripts.Core.Model
{
    public class MarketEvent
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public List<String> AffectedMaterials { get; set; }
        public Double PriceImpact { get; set; }
        public Double Duration { get; set; }
    }
}
