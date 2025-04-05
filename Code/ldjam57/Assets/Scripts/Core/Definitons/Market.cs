using System;
using System.Collections.Generic;

using GameFrame.Core.Definitions;

namespace Assets.Scripts.Core.Definitons
{
    public class Market : BaseDefinition
    {
        public String Name { get; set; }
        public Double Factor { get; set; }
        public List<MaterialValueDefinition> MaterialMarketValues { get; set; }
    }
}
