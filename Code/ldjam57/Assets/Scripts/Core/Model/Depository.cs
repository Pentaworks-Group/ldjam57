using System;

using Assets.Scripts.Core.Definitons;

using GameFrame.Core.Math;

namespace Assets.Scripts.Core.Model
{
    public class Depository
    {
        public DepositoryDefinition Definition { get; set; }
        public Mineral Mineral { get; set; }
        public Point2 Position { get; set; }
        public Double Capacity { get; set; }
        public Double Value { get; set; }
    }
}
