using System;
using System.Collections.Generic;

namespace Assets.Scripts.Core.Model
{
    public class World
    {
        public List<Tile> Tiles { get; set; }
        public Double Seed { get; set; }
        public List<Depository> Depositories { get; set; }
        public Headquarters Headquarters { get; set; }
    }
}
