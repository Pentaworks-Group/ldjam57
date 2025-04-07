using System;
using System.Collections.Generic;

using GameFrame.Core.Definitions;

namespace Assets.Scripts.Core.Definitons
{
    public class MarketEventDefinition : BaseDefinition
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public List<String> AffectedMaterials { get; set; }
        public Double PriceImpact { get; set; }
        public Double Duration { get; set; }
    }
}
