using System;
using System.Collections.Generic;

namespace Assets.Scripts.Core.Definitons
{
    public class WorldDefinition
    {
        public List<MaterialDefinition> Materials { get; set; }
        public Double? Seed { get; set; }
        public Int32 MaxWidth { get; set; }
        public HeadquartersDefinition Headquarters { get; set; }
    }
}
