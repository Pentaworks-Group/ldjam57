using System;

using GameFrame.Core.Definitions;

namespace Assets.Scripts.Core.Definitons
{
    public class MaterialDefinition : BaseDefinition
    {
        public String Name { get; set; }
        public GameFrame.Core.Math.Range SpawnRange { get; set; }
        public GameFrame.Core.Media.Color? Color { get; set; }
        public Double? Weight { get; set; }
        public Double? MiningSpeedFactor { get; set; }
    }
}
