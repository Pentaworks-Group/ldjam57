using System;
using System.Collections.Generic;

namespace Assets.Scripts.Core.Model
{
    public class World
    {
        public Definitons.WorldDefinition Definition { get; set; }
        public List<Mineral> Minerals { get; set; }
        public Int32 Width { get; set; }
        public Double Seed { get; set; }
        public List<Tile> Tiles { get; set; }
        public List<Depository> Depositories { get; set; }
        public Headquarters Headquarters { get; set; }
    }
}
