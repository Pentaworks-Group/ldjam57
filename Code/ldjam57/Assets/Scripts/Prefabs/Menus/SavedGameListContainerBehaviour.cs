using System.Linq;

using Assets.Scripts.Core.Persistence;

namespace Assets.Scripts.Prefabs.Menus
{
    public class SavedGameListContainerBehaviour : GameFrame.Core.UI.List.ListContainerBehaviour<SavedGamePreview>
    {
        public override void CustomStart()
        {
            UpdateList();
        }

        public void UpdateList()
        {
            var savedGames = Base.Core.Game.GetSavedGamePreviews().OrderBy(k => k.Key).Select(k => k.Value).ToList();

            SetContentList(savedGames);
        }
    }
}
