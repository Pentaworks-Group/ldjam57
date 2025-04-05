using System.Collections.Generic;

using Assets.Scripts.Core.Model;

namespace Assets.Scripts.Core
{
    public class GameState : GameFrame.Core.GameState
    {
        public Assets.Scripts.Core.Definitons.GameMode GameMode { get; set; }
        public Bank Bank { get; set; }
        public Market Market { get; set; }
        public Headquarters Headquarters { get; set; }
        public World World { get; set; }
        public Inventory Inventory { get; set; }
        public List<MiningTool> AvailableMiningTools { get; set; }
        public List<Transport> AvailableHorizontalTransports { get; set; }
        public List<Transport> AvailableVerticalTransports { get; set; }
    }
}
