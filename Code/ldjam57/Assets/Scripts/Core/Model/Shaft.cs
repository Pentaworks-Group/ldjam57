using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace Assets.Scripts.Core.Model
{
    public class Shaft
    {
        public GameFrame.Core.Math.Point2 Position { get; set; }

        public float UsedCapacity { get; set; } = 0f;
    }
}
