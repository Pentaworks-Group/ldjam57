using System;
using System.Collections.Generic;

using GameFrame.Core.Definitions;

namespace Assets.Scripts.Core.Definitons
{
    public class HeadquartersDefinition : BaseDefinition
    {
        public GameFrame.Core.Math.Vector2? Position { get; set; }
        public List<String> AvailableSprites { get; set; }
    }
}
