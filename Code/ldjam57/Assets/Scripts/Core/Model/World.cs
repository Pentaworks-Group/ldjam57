using System;
using System.Collections.Generic;

namespace Assets.Scripts.Core.Model
{
    public class World
    {
        public Definitons.WorldDefinition Definition { get; set; }
        public List<Mineral> Minerals { get; set; } = new List<Mineral>();
        public Mineral DefaultMineral { get; set; }
        public Int32 Width { get; set; }
        public Single Seed { get; set; }
        public List<Tile> Tiles { get; set; } = new List<Tile>();
        public List<Depository> Depositories { get; set; } = new List<Depository>();
        public Headquarters Headquarters { get; set; }
    }
}
