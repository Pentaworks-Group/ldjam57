using System;
using System.Collections.Generic;

namespace Assets.Scripts.Core.Model
{
    public class Tile
    {
        public GameFrame.Core.Math.Point2 Position { get; set; }
        public Dictionary<String, Double> MineralAmounts { get; set; }
    }
}
