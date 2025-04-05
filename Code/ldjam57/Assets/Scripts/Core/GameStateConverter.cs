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

        public GameStateConverter(GameMode mode)
        {
            this.mode = mode;
        }

        public GameState Convert()
        {
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
                gameState.Market = ConvertMarket();
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
                gameState.AvailableVerticalTransports = new List<Transport>();

                ConvertTransports(mode.AvailableVerticalTransports, gameState.AvailableVerticalTransports);
            }
            else
            {
                throw new ArgumentNullException(nameof(GameMode.AvailableMiningTools), "GameMode.AvailableVerticalTransports has no entries.");
            }

            if (mode.AvailableHorizontalTransports?.Count > 0)
            {
                gameState.AvailableHorizontalTransports = new List<Transport>();

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
                Tiles = new List<Tile>(),
                Headquarters = ConvertHeadquarters(mode.World.Headquarters),
                Depositories = new List<Depository>()
            };

            if (mode.World.Seed.HasValue)
            {
                world.Seed = mode.World.Seed.Value;
            }
            else
            {
                world.Seed = new Random().NextDouble();
            }

            ConvertDepositories(world.Depositories);

            return world;
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

        private Market ConvertMarket()
        {
            var market = new Market()
            {
                MaterialValues = new List<MarketValue>()
            };

            foreach (var materialValue in mode.Market.MaterialValues)
            {
                var newMarketValue = new MarketValue()
                {
                    Material = materialValue.Material,
                    Value = materialValue.Value.GetValueOrDefault(),
                };

                market.MaterialValues.Add(newMarketValue);
            }

            return market;
        }

        private void ConvertMiningTools(GameState gameState)
        {
            gameState.AvailableMiningTools = new List<MiningTool>();

            foreach (var definition in mode.AvailableMiningTools)
            {
                var miningTool = new MiningTool()
                {
                    Name = definition.Name,
                    Sprite = definition.Sprite,
                    Capacity = definition.Capacity.GetValueOrDefault(),
                    SpeedFactor= definition.SpeedFactor.GetValueOrDefault(1),
                    Size = definition.Size.GetValueOrDefault(new GameFrame.Core.Math.Vector2(1,1)),
                    IsUnlocked = definition.IsUnlocked.GetValueOrDefault(),
                    IsUnlockable = definition.IsUnlockable.GetValueOrDefault(),
                    UnlockCost= definition.UnlockCost.GetValueOrDefault(),
                    PurchaseCost= definition.PurchaseCost.GetValueOrDefault(),
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
                    Definition = definition,

                };

                targetList.Add(transport);
            }
        }

        private Inventory ConvertInventory()
        {
            var inventory = new Inventory();

            return inventory;
        }
    }
}