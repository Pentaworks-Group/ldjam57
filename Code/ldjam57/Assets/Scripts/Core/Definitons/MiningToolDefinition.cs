using System;
using System.Collections.Generic;

using GameFrame.Core.Definitions;

namespace Assets.Scripts.Core.Definitons
{
    public class MiningToolDefinition : BaseDefinition
    {
        public String Name { get; set; }
        public GameFrame.Core.Math.Vector2? Size { get; set; }
        public List<String> AvailableSprites { get; set; }
        public Double? MiningSpeedFactor { get; set; }
        public Double? Capacity { get; set; }
    }
}
