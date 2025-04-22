using System;

namespace Assets.Scripts.Core.Persistence
{
    public class SavedGamePreview : GameFrame.Core.Persistence.SavedGamePreview<GameState>
    {
        public DateTime SavedOn { get; set; }
        public DateTime StartedOn { get; set; }
        public Double Money { get; set; }
        public Int32 Depth { get; set; }
        public Double TimeElapsed { get; set; }

        public override void Init(GameState gameState, String key)
        {
            base.Init(gameState, key);

            this.SavedOn = gameState.SavedOn;
            this.StartedOn = gameState.CreatedOn;
            this.Money = gameState.Bank.Credits;
            this.Depth = gameState.World.MaxDepth;
            this.TimeElapsed = gameState.TimeElapsed;
        }
    }
}
