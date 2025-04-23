using System;
using System.Collections.Generic;

using GameFrame.Core.Math;

namespace Assets.Scripts.Core.Model
{
    public class Transporter
    {
        public Transport Transport { get; set; }
        public Point2 Position { get; set; }
        public Direction Direction { get; set; }
        public Double Tick { get; set; }
        public Dictionary<String, MineralAmount> StoredAmount { get; set; } = new Dictionary<String, MineralAmount>();
        public Boolean IsActive { get; set; }
    }
}
