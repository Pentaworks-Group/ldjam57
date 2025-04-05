using System;
using System.Collections.Generic;

namespace Assets.Scripts.Core.Definitons
{
    public class WorldDefinition
    {
        public Double? Seed { get; set; }
        public Int32? Width { get; set; }
        public HeadquartersDefinition Headquarters { get; set; }
        public List<MaterialDefinition> Materials { get; set; }
        public List<DepositoryDefinition> Depositories { get; set; }
    }
}
