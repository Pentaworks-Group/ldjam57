using System;
using System.Collections.Generic;

namespace Assets.Scripts.Constants
{
    public class SceneNames
    {
        public const String MainMenu = Scenes.MainMenuName;
        public const String Credits = Scenes.CreditsName;
        public const String Options = Scenes.OptionsName;
        public const String Game = Scenes.GameName;
        public const String MovementTest = Scenes.MovementTestName;
        public const String TerrainTest = Scenes.TerrainTestName;

        public static List<String> scenes = new() { Scenes.MainMenuName, Scenes.OptionsName, Scenes.CreditsName, Scenes.GameName };
        public static List<String> scenesDevelopment = new() { };
    }
}