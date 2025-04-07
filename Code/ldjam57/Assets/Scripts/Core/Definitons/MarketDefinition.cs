using System;
using System.Collections.Generic;

using GameFrame.Core.Definitions;

namespace Assets.Scripts.Core.Definitons
{
    public class MarketDefinition : BaseDefinition
    {
        public String Name { get; set; }
        public Double? Factor { get; set; }
        public List<MineralValueDefinition> MineralValues { get; set; }
        public Double? UpdateInterval { get; set; }
        public Double? Volatility { get; set; }
        public Boolean? EnableRandomEvents { get; set; }
        public Double? EventProbability { get; set; }
        public Double? EventImpactMultiplier { get; set;}
        public List<MarketEventDefinition> Events { get; set; }
    }
}
