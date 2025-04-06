using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Constants;
using UnityEngine;

namespace Assets.Scripts.Scenes.Menues
{
    public class MainMenuBaseBehaviour : BaseMenuBehaviour
    {
        private void Start()
        {
            var introAudio = GameFrame.Base.Resources.Manager.Audio.Get("Intro");

            var backgroundAudioClips = new List<AudioClip>()
            {
                GameFrame.Base.Resources.Manager.Audio.Get("Menu_empty"),
                GameFrame.Base.Resources.Manager.Audio.Get("Menu_empty"),
                GameFrame.Base.Resources.Manager.Audio.Get("Menu_empty"),
                GameFrame.Base.Resources.Manager.Audio.Get("Menu_1"),
                GameFrame.Base.Resources.Manager.Audio.Get("Menu_2")
            };
            GameFrame.Base.Audio.Background.ReplaceClips(backgroundAudioClips);
            GameFrame.Base.Audio.Background.PlayTransition(introAudio, backgroundAudioClips);
        }

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
