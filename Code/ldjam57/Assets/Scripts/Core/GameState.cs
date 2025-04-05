namespace Assets.Scripts.Core
{
    public class GameState : GameFrame.Core.GameState
    {
        public Assets.Scripts.Core.Definitons.GameMode GameMode { get; set; }
        public Bank Bank { get; set; }
        public Headquarters Headquarters { get; set; }
        public Storage Storage { get; set; }
    }
}
