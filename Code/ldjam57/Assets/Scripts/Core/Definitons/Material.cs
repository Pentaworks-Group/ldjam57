using System;

using GameFrame.Core.Definitions;

namespace Assets.Scripts.Core.Definitons
{
    public class Material : BaseDefinition
    {
        public String Name { get; set; }
        public GameFrame.Core.Math.Range SpawnRange { get; set; }
        public Double Weight { get; set; }
        public Single MiningSpeedFactor { get; set; }
    }
}
