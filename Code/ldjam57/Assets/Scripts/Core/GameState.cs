using System;
using System.Collections.Generic;

using Assets.Scripts.Core.Model;

namespace Assets.Scripts.Core
{
    public class GameState : GameFrame.Core.GameState
    {
        public Assets.Scripts.Core.Definitons.GameMode GameMode { get; set; }
        public Bank Bank { get; set; }
        public Market Market { get; set; }
        public World World { get; set; }
        public Inventory Inventory { get; set; }
        public List<MiningTool> AvailableMiningTools { get; set; } = new List<MiningTool>();
        public List<Transport> AvailableHorizontalTransports { get; set; } = new List<Transport>();
        public List<Transport> AvailableVerticalTransports { get; set; } = new List<Transport>();
        public List<Digger> ActiveDiggers { get; set; } = new List<Digger>();
        public Double TimeElapsed { get; set; }

    }
}
