using System;
using System.Collections.Generic;

using GameFrame.Core;

namespace Assets.Scripts.Constants
{
    public static class Scenes
    {
        public const String MainMenuName = "MainMenuScene";
        private static Scene mainMenu;
        public static Scene MainMenu
        {
            get
            {
                if (mainMenu == default)
                {
                    mainMenu = new Scene()
                    {
                        Name = MainMenuName
                    };
                }

                return mainMenu;
            }
        }

        public const String CreditsName = "CreditsScene";
        private static Scene credits;
        public static Scene Credits
        {
            get
            {
                if (credits == default)
                {
                    credits = new Scene()
                    {
                        Name = CreditsName,
                        AmbienceClips = new List<String>()
                        {
                            "WoodSound"
                        },
                    };
                }

                return credits;
            }
        }

        public const String OptionsName = "OptionsScene";
        private static Scene options;
        public static Scene Options
        {
            get
            {
                if (options == default)
                {
                    options = new Scene()
                    {
                        Name = OptionsName
                    };
                }

                return options;
            }
        }

        public const String GameName = "GameScene";
        private static Scene game;
        public static Scene Game
        {
            get
            {
                if (game == default)
                {
                    game = new Scene()
                    {
                        Name = GameName,
                        AmbienceClips = new List<String>()
                        {
                            "WoodSound"
                        },
                        BackgroundClips = new List<String>()
                        {
                            "Background"
                        }
                    };
                }

                return game;
            }
        }

        public const String MovementTestName = "MovementTestScene";
        private static Scene movementTest;
        public static Scene MovementTest
        {
            get
            {
                if (movementTest == default)
                {
                    movementTest = new Scene()
                    {
                        Name = MovementTestName
                    };
                }

                return movementTest;
            }
        }

        public const String TerrainTestName = "TerrainTestScene";
        private static Scene terrainTest;
        public static Scene TerrainTest
        {
            get
            {
                if (terrainTest == default)
                {
                    terrainTest = new Scene()
                    {
                        Name = TerrainTestName
                    };
                }

                return terrainTest;
            }
        }

        public static IList<Scene> GetAll()
        {
            return new List<Scene>()
            {
                MainMenu,
                Credits,
                Options,
                Game,
                MovementTest,
                TerrainTest
            };
        }
    }
}
