﻿using System;
using System.Collections.Generic;

namespace Assets.Scripts.Core.Model
{
    public class Tile
    {
        public GameFrame.Core.Math.Vector2 Position { get; set; }
        public Dictionary<String, Double> MaterialAmounts { get; set; }
    }
}
