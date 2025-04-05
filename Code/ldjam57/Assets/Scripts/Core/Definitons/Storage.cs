using System;
using System.Collections.Generic;

using GameFrame.Core.Definitions;
using GameFrame.Core.Math;

namespace Assets.Scripts.Core.Definitons
{
    public class Storage : BaseDefinition
    {
        public String Name { get; set; }
        public Vector2 Poition { get; set; }
        public List<String> AvailableSprites { get; set; }
        public Single Capacity { get; set; }
        public Material Material { get; set; }
    }
}
