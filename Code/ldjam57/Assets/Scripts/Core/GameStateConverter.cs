using Assets.Scripts.Constants;
using Assets.Scripts.Core.Definitons;

namespace Assets.Scripts.Core
{
    internal class GameStateConverter
    {
        private GameMode mode;

        public GameStateConverter(GameMode mode)
        {
            this.mode = mode;
        }

        public GameState Convert()
        {
            var gameState = new GameState()
            {
                GameMode = mode,
                CurrentScene = SceneNames.Game
            };


            return gameState;
        }
    }
}