using System;
using System.Collections.Generic;

using Assets.Scripts.Core.Definitons.Inventories;
using Assets.Scripts.Core.Model.Inventories;

using GameFrame.Core.Definitions.Loaders;

namespace Assets.Scripts.Core.Definitons.Loaders
{
    public class GameModeLoader : BaseLoader<GameMode>
    {
        private readonly DefinitionCache<MarketDefinition> marketCache;
        private readonly DefinitionCache<MineralDefinition> mineralCache;
        private readonly DefinitionCache<TransportDefinition> transportCache;
        private readonly DefinitionCache<MiningToolDefinition> miningToolCache;

        public GameModeLoader(DefinitionCache<GameMode> targetCache, DefinitionCache<MineralDefinition> mineralCache, DefinitionCache<MiningToolDefinition> miningToolCache, DefinitionCache<TransportDefinition> transportCache, DefinitionCache<MarketDefinition> marketCache) : base(targetCache)
        {
            this.marketCache = marketCache;
            this.mineralCache = mineralCache;
            this.transportCache = transportCache;
            this.miningToolCache = miningToolCache;
        }

        protected override void OnDefinitionsLoaded(List<GameMode> definitions)
        {
            _ = new GameMode() { IsReferenced = true };

            if (definitions?.Count > 0)
            {
                foreach (var loadedGameMode in definitions)
                {
                    var newGameMode = new GameMode()
                    {
                        Reference = loadedGameMode.Reference,
                        Name = loadedGameMode.Name,
                        Market = CheckItem(loadedGameMode.Market, marketCache),
                        AvailableMiningTools = new List<MiningToolDefinition>(),
                        AvailableVerticalTransports = new List<TransportDefinition>(),
                        AvailableHorizontalTransports = new List<TransportDefinition>()
                    };                        

                    CheckItems(loadedGameMode.AvailableMiningTools, newGameMode.AvailableMiningTools, miningToolCache);
                    CheckItems(loadedGameMode.AvailableVerticalTransports, newGameMode.AvailableVerticalTransports, transportCache);
                    CheckItems(loadedGameMode.AvailableHorizontalTransports, newGameMode.AvailableHorizontalTransports, transportCache);

                    if (loadedGameMode.World != default)
                    {
                        newGameMode.World = new WorldDefinition()
                        {
                            Seed = loadedGameMode.World.Seed,
                            Width = loadedGameMode.World.Width,
                            Minerals = new List<MineralDefinition>(),
                            Headquarters = loadedGameMode.World.Headquarters,
                            Depositories = new List<DepositoryDefinition>()
                        };

                        CheckItems(loadedGameMode.World.Minerals, newGameMode.World.Minerals, this.mineralCache);

                        if (loadedGameMode.World.Depositories != default)
                        {
                            foreach (var loadedDepository in loadedGameMode.World.Depositories)
                            {
                                var newDepository = new DepositoryDefinition()
                                {
                                    Mineral = CheckItem(loadedDepository.Mineral, this.mineralCache),
                                    Capacity = loadedDepository.Capacity,
                                    Position = loadedDepository.Position,
                                    Reference = loadedDepository.Reference,
                                    Sprite = loadedDepository.Sprite,
                                };

                                newGameMode.World.Depositories.Add(newDepository);
                            }
                        }
                    }

                    if (loadedGameMode.Bank != default)
                    {
                        newGameMode.Bank = new BankDefinition()
                        {
                            Credits = loadedGameMode.Bank.Credits,
                        };
                    }

                    if (loadedGameMode.Inventory != default)
                    {
                        newGameMode.Inventory = new InventoryDefinition()
                        {
                            MiningTools = new List<MiningToolDefinitionInventoryItem>(),
                            HorizontalTransports = new List<TransportDefinitionInventoryItem>(),
                            VerticalTransports = new List<TransportDefinitionInventoryItem>(),
                        };

                        if (loadedGameMode.Inventory.MiningTools != default)
                        {
                            CheckMiningToolInventoryItems(loadedGameMode.Inventory.MiningTools, newGameMode.Inventory.MiningTools, this.miningToolCache);
                        }

                        if (loadedGameMode.Inventory.HorizontalTransports != default)
                        {
                            CheckTransportInventoryItems(loadedGameMode.Inventory.HorizontalTransports, newGameMode.Inventory.HorizontalTransports, this.transportCache);
                        }

                        if (loadedGameMode.Inventory.VerticalTransports != default)
                        {
                            CheckTransportInventoryItems(loadedGameMode.Inventory.VerticalTransports, newGameMode.Inventory.VerticalTransports, this.transportCache);
                        }
                    }

                    targetCache[loadedGameMode.Reference] = newGameMode;
                }
            }
        }

        private void CheckMiningToolInventoryItems(List<MiningToolDefinitionInventoryItem> loadedItems, List<MiningToolDefinitionInventoryItem> targetItems, DefinitionCache<MiningToolDefinition> cache)
        {
            foreach (var transportDefinitionInventoryItem in loadedItems)
            {
                var transportInventoryItem = new MiningToolDefinitionInventoryItem()
                {
                    MiningTool = CheckItem(transportDefinitionInventoryItem.MiningTool, cache),
                    Amount = transportDefinitionInventoryItem.Amount
                };

                targetItems.Add(transportInventoryItem);
            }
        }

        private void CheckTransportInventoryItems(List<TransportDefinitionInventoryItem> loadedItems, List<TransportDefinitionInventoryItem> targetItems, DefinitionCache<TransportDefinition> cache)
        {
            foreach (var loadedItem in loadedItems)
            {
                var targetItem = new TransportDefinitionInventoryItem()
                {
                    Transport = CheckItem(loadedItem.Transport, cache),
                    Amount = loadedItem.Amount
                };

                targetItems.Add(targetItem);
            }
        }
    }
}
