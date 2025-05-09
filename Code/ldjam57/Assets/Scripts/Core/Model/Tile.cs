﻿using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace Assets.Scripts.Core.Model
{
    public class Tile
    {
        public GameFrame.Core.Math.Point2 Position { get; set; }
        public List<MineralAmount> MineralAmounts { get; set; }

        public float DigingProgress { get; set; } = 0f;

        public float SpeedFactor { get; set; } = 1f;
    }
}
