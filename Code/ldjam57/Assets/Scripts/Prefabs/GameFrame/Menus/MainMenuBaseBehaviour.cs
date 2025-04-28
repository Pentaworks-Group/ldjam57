using System.Collections.Generic;
using System.Runtime.InteropServices;

using Assets.Scripts.Constants;

using UnityEngine;

namespace Assets.Scripts.Scenes.Menues
{
    public class MainMenuBaseBehaviour : BaseMenuBehaviour
    {
        [DllImport("__Internal")]
        private static extern void Quit();

        [SerializeField]
        private GameObject quitButton;

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

        public void Awake()
        {
            if (SystemInfo.deviceType != DeviceType.Handheld)
            {
                quitButton.SetActive(true);
            }
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
        
        public void ShowSavedGames()
        {
            Base.Core.Game.PlayButtonSound();
            Base.Core.Game.ChangeScene(SceneNames.SavedGames);
        }

        public void ShowCredits()
        {
            Base.Core.Game.PlayButtonSound();
            Base.Core.Game.ChangeScene(SceneNames.Credits);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
                        Quit();
#elif UNITY_STANDALONE
                        Application.Quit();            
#endif
        }

        public void OpenItch()
        {
            Application.OpenURL("https://pentaworks.itch.io/");
        }
    }
}
