using Assets.Scripts.Constants;

using UnityEngine;

namespace Assets.Scripts.Scenes.Menues
{
    public class MainMenuBaseBehaviour : BaseMenuBehaviour
    {
        public void Play()
        {
            Base.Core.Game.PlayButtonSound();
            Base.Core.Game.Start();
        }

        public void ShowOptions()
        {
            Base.Core.Game.PlayButtonSound();
            Base.Core.Game.ChangeScene(SceneNames.Options);
        }

        public void ShowCredits()
        {
            Base.Core.Game.PlayButtonSound();
            Base.Core.Game.ChangeScene(SceneNames.Credits);
        }

        public void OpenItch()
        {
            Application.OpenURL("https://pentaworks.itch.io/");
        }
    }
}
