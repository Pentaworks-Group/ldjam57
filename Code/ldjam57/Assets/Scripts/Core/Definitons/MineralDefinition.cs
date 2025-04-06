using System;

using GameFrame.Core.Definitions;

namespace Assets.Scripts.Core.Definitons
{
    public class MineralDefinition : BaseDefinition
    {
        public String Name { get; set; }
        public Boolean? IsDefault { get; set; }
        public Boolean? IsMetallic { get; set; }
        public GameFrame.Core.Math.Range SpawnRange { get; set; }
        public GameFrame.Core.Math.Range SeedRange { get; set; }
        public GameFrame.Core.Media.Color? Color { get; set; }
        public Double? Weight { get; set; }
        public Double? MiningSpeedFactor { get; set; }
    }
}
