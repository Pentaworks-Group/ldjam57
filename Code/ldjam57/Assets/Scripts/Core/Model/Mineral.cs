using System;

namespace Assets.Scripts.Core.Model
{
    public class Mineral
    {
        public String Reference { get; set; }
        public String Name { get; set; }
        public GameFrame.Core.Math.Range SpawnRange { get; set; }
        public Int32 Seed { get; set; }
        public GameFrame.Core.Media.Color Color { get; set; }
        public Double Weight { get; set; }
        public Double MiningSpeedFactor { get; set; }
    }
}
