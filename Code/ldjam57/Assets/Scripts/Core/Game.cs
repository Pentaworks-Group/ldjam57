using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Core.Definitons;
using Assets.Scripts.Core.Definitons.Loaders;
using Assets.Scripts.Core.Persistence;

using GameFrame.Core.Definitions.Loaders;
using GameFrame.Core.Extensions;

using UnityEngine;

namespace Assets.Scripts.Core
{
    public class Game : GameFrame.Core.SaveableGame<GameState, PlayerOptions, SavedGamePreview>
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
                BackgroundVolume = 0.5f,
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
            //this.buttonAudioClips.Add(GameFrame.Base.Resources.Manager.Audio.Get("Buzz_1"));
            //this.buttonAudioClips.Add(GameFrame.Base.Resources.Manager.Audio.Get("Buzz_2"));
        }

        protected override IEnumerator LoadDefintions()
        {
            var mineralCache = new DefinitionCache<MineralDefinition>();
            var miningToolCache = new DefinitionCache<MiningToolDefinition>();
            var transportCache = new DefinitionCache<TransportDefinition>();
            var marketCache = new DefinitionCache<MarketDefinition>();

            yield return new DefinitionLoader<MineralDefinition>(mineralCache).LoadDefinitions("Minerals.json");
            yield return new DefinitionLoader<MiningToolDefinition>(miningToolCache).LoadDefinitions("MiningTools.json");
            yield return new DefinitionLoader<TransportDefinition>(transportCache).LoadDefinitions("Transports.json");
            yield return new MarketDefinitionLoader(marketCache, mineralCache).LoadDefinitions("Markets.json");
            yield return new GameModeLoader(this.gameModeCache, mineralCache, miningToolCache, transportCache, marketCache).LoadDefinitions("GameModes.json");
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
