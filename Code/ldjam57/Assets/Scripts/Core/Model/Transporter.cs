using Assets.Scripts.Core.Model;
using GameFrame.Core.Math;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Core.Model
{
    public class Transporter
    {
        public Transport Transport { get; set; }
        public Point2 Position { get; set; }
        public Direction Direction { get; set; }
        public Double Tick { get; set; }
        public Dictionary<Mineral, Double> StoredAmount { get; set; } = new Dictionary<Mineral, Double>();
    }
}
