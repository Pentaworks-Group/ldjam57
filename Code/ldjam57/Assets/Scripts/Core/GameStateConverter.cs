using System;
using System.Collections.Generic;

using Assets.Scripts.Constants;
using Assets.Scripts.Core.Definitons;
using Assets.Scripts.Core.Model;

using GameFrame.Core.Extensions;

namespace Assets.Scripts.Core
{
    internal class GameStateConverter
    {
        private readonly GameMode mode;

        private IDictionary<String, Mineral> mineralMap;

        public GameStateConverter(GameMode mode)
        {
            this.mode = mode;
        }

        public GameState Convert()
        {
            mineralMap = new Dictionary<String, Mineral>();

            var gameState = new GameState()
            {
                GameMode = mode,
                CurrentScene = SceneNames.Game,
                Bank = new Bank(),
            };

            if (mode.Bank != default)
            {
                gameState.Bank.Credits = mode.Bank.Credits.GetValueOrDefault();
            }

            if (mode.World != default)
            {
                gameState.World = ConvertWorld();
            }
            else
            {
                throw new ArgumentNullException(nameof(GameMode.World), "GameMode.World is null. This should not happen!");
            }

            if (mode.Market != default)
            {
                gameState.Market = ConvertMarket(gameState.World);
            }
            else
            {
                throw new ArgumentNullException(nameof(GameMode.Market), "GameMode.Market is null. This should not happen!");
            }

            if (mode.AvailableMiningTools?.Count > 0)
            {
                ConvertMiningTools(gameState);
            }
            else
            {
                throw new ArgumentNullException(nameof(GameMode.AvailableMiningTools), "GameMode.AvailableMiningTools has no entries. Cant dig without tools..");
            }

            if (mode.AvailableVerticalTransports?.Count > 0)
            {
                ConvertTransports(mode.AvailableVerticalTransports, gameState.AvailableVerticalTransports);
            }
            else
            {
                throw new ArgumentNullException(nameof(GameMode.AvailableMiningTools), "GameMode.AvailableVerticalTransports has no entries.");
            }

            if (mode.AvailableHorizontalTransports?.Count > 0)
            {
                ConvertTransports(mode.AvailableHorizontalTransports, gameState.AvailableHorizontalTransports);
            }
            else
            {
                throw new ArgumentNullException(nameof(GameMode.AvailableMiningTools), "GameMode.AvailableHorizontalTransports has no entries.");
            }

            gameState.Inventory = ConvertInventory();

            return gameState;
        }

        private World ConvertWorld()
        {
            var world = new World()
            {
                Definition = mode.World,
                Width = mode.World.Width.GetValueOrDefault(64),
                Headquarters = ConvertHeadquarters(mode.World.Headquarters),
            };

            ConvertMinerals(world);

            if (mode.World.Seed.HasValue)
            {
                world.Seed = mode.World.Seed.Value;
            }
            else
            {
                world.Seed = UnityEngine.Random.value * 100;
            }

            var tileGenerator = new TileGenerator(world);

            world.Tiles.AddRange(tileGenerator.GenerateRootTiles());

            ConvertDepositories(world.Depositories);

            return world;
        }

        private void ConvertMinerals(World world)
        {
            if (mode.World.Minerals.Count > 0)
            {
                foreach (var mineralDefinition in mode.World.Minerals)
                {
                    var mineral = new Mineral()
                    {
                        Reference = mineralDefinition.Reference,
                        Name = mineralDefinition.Name,
                        IsDefault = mineralDefinition.IsDefault.GetValueOrDefault(),
                        MiningSpeedFactor = mineralDefinition.MiningSpeedFactor.GetValueOrDefault(1),
                        Seed = mineralDefinition.SeedRange.GetRandomInt(),
                        Color = mineralDefinition.Color.GetValueOrDefault(new GameFrame.Core.Media.Color(1, 1, 1)),
                        SpawnRange = mineralDefinition.SpawnRange?.Copy(),
                        Weight = mineralDefinition.Weight.GetValueOrDefault(1),
                    };

                    mineralMap[mineralDefinition.Reference] = mineral;

                    if (mineral.IsDefault)
                    {
                        if (world.DefaultMineral == default)
                        {
                            world.DefaultMineral = mineral;
                        }
                    }

                    world.Minerals.Add(mineral);
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(mode.World.Minerals), "Minerals may not be empty!");
            }
        }

        private Headquarters ConvertHeadquarters(Definitons.HeadquartersDefinition headquartersDefinition)
        {
            var headquarters = new Headquarters()
            {
                Position = headquartersDefinition.Position.GetValueOrDefault(),
                Sprite = headquartersDefinition.AvailableSprites.GetRandomEntry()
            };

            return headquarters;
        }

        private void ConvertDepositories(List<Depository> depositoryList)
        {
            if (mode.World.Depositories?.Count > 0)
            {
                foreach (var definition in mode.World.Depositories)
                {
                    var depository = new Depository()
                    {
                        Definition = definition
                    };

                    depositoryList.Add(depository);
                }
            }
        }

        private Market ConvertMarket(World world)
        {
            var market = new Market()
            {
                MineralValues = new List<MineralMarketValue>()
            };

            foreach (var mineralMarketValue in mode.Market.MineralValues)
            {
                var newMarketValue = new MineralMarketValue()
                {
                    Mineral = mineralMap[mineralMarketValue.Mineral.Reference],
                    Value = mineralMarketValue.Value.GetValueOrDefault(),
                };

                market.MineralValues.Add(newMarketValue);
            }

            return market;
        }

        private void ConvertMiningTools(GameState gameState)
        {
            foreach (var definition in mode.AvailableMiningTools)
            {
                var miningTool = new MiningTool()
                {
                    Name = definition.Name,
                    Sprite = definition.Sprite,
                    Capacity = definition.Capacity.GetValueOrDefault(),
                    SpeedFactor = definition.SpeedFactor.GetValueOrDefault(1),
                    Size = definition.Size.GetValueOrDefault(new GameFrame.Core.Math.Vector2(1, 1)),
                    IsUnlocked = definition.IsUnlocked.GetValueOrDefault(),
                    IsUnlockable = definition.IsUnlockable.GetValueOrDefault(),
                    UnlockCost = definition.UnlockCost.GetValueOrDefault(),
                    PurchaseCost = definition.PurchaseCost.GetValueOrDefault(),
                };

                gameState.AvailableMiningTools.Add(miningTool);
            }
        }

        private void ConvertTransports(List<TransportDefinition> definitions, List<Transport> targetList)
        {
            foreach (var definition in definitions)
            {
                var transport = new Transport()
                {
                    Name = definition.Name,
                    Sprite = definition.Sprite,
                    Speed = definition.Speed.GetValueOrDefault(1),
                    Capacity = definition.Capacity.GetValueOrDefault(),
                    Size = definition.Size.GetValueOrDefault(new GameFrame.Core.Math.Vector2(1, 1)),
                    IsUnlocked = definition.IsUnlocked.GetValueOrDefault(),
                    IsUnlockable = definition.IsUnlockable.GetValueOrDefault(),
                    UnlockCost = definition.UnlockCost.GetValueOrDefault(),
                    PurchaseCost = definition.PurchaseCost.GetValueOrDefault(),
                };

                targetList.Add(transport);
            }
        }

        private Inventory ConvertInventory()
        {
            var inventory = new Inventory();

            return inventory;
        }

        private List<Digger> ConvertDiggers()
        {
            var diggers = new List<Digger>();

            return diggers;
        }
    }
}