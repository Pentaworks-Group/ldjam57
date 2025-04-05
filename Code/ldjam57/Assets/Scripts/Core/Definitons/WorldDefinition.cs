using System;
using System.Collections.Generic;

namespace Assets.Scripts.Core.Definitons
{
    public class WorldDefinition
    {
        public List<Material> Materials { get; set; }
        public Double? Seed { get; set; }
        public Int32 MaxWidth { get; set; }
        public Headquarters Headquarters { get; set; }
    }
}
