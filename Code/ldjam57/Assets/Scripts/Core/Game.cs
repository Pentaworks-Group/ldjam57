using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Core.Definitons;

using GameFrame.Core.Definitions.Loaders;
using GameFrame.Core.Extensions;

using UnityEngine;

namespace Assets.Scripts.Core
{
    public class Game : GameFrame.Core.Game<GameState, PlayerOptions>
    {
        private readonly DefinitionCache<GameMode> gameModeCache = new DefinitionCache<GameMode>();

        private readonly List<UnityEngine.AudioClip> buttonAudioClips = new List<AudioClip>();

        public override void PlayButtonSound()
        {
            GameFrame.Base.Audio.Effects.Play(this.buttonAudioClips.GetRandomEntry());
        }

        protected override GameState InitializeGameState()
        {
            var mode = this.gameModeCache.Values.First();

            var ganeStateConverter = new GameStateConverter(mode);

            var gameState = ganeStateConverter.Convert();

            return gameState;
        }

        protected override PlayerOptions InitializePlayerOptions()
        {
            Debug.LogFormat("System: {0}", SystemInfo.deviceType);

            var showTouchPads = false;

            if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                showTouchPads = true;
            }

            return new PlayerOptions()
            {
                EffectsVolume = 0.9f,
                AmbienceVolume = 0.1f,
                BackgroundVolume = 0.05f,
                ShowTouchPads = showTouchPads
            };
        }

        protected override void RegisterScenes()
        {
            RegisterScenes(Constants.Scenes.GetAll());
        }

        protected override void InitializeAudioClips()
        {
            InitializeButtonEffects();
        }

        private void InitializeButtonEffects()
        {
            this.buttonAudioClips.Add(GameFrame.Base.Resources.Manager.Audio.Get("Buzz_1"));
            this.buttonAudioClips.Add(GameFrame.Base.Resources.Manager.Audio.Get("Buzz_2"));
        }

        protected override IEnumerator LoadDefintions()
        {
            yield return null;
            Debug.Log("loaded definitions");
        }

        protected override void OnGameStartup()
        { }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void GameStart()
        {
            Base.Core.Game.Startup();
        }
    }
}
