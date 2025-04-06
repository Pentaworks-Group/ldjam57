using System;
using System.Collections.Generic;
using GameFrame.Core.Math;

namespace Assets.Scripts.Core.Model
{
    public class Digger
    {
        public MiningTool MiningTool { get; set; }
        public Point2 Position { get; set; }
        public Direction Direction { get; set; }        
        public Double Tick { get; set; }
        public Boolean IsMining { get; set; }
        public Dictionary<Mineral, Double> MinedAmount { get; set; } = new Dictionary<Mineral, Double>();
    }
}
