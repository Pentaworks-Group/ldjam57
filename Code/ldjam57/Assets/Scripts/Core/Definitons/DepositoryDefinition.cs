using System;

using GameFrame.Core.Definitions;
using GameFrame.Core.Math;

namespace Assets.Scripts.Core.Definitons
{
    public class DepositoryDefinition : BaseDefinition
    {
        public MaterialDefinition Material { get; set; }
        public Vector2? Poition { get; set; }
        public String Sprite { get; set; }
        public Double? Capacity { get; set; }
    }
}
