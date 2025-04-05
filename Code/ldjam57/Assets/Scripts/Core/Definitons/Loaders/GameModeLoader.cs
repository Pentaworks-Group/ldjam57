using System.Collections.Generic;

using GameFrame.Core.Definitions.Loaders;

namespace Assets.Scripts.Core.Definitons.Loaders
{
    public class GameModeLoader : BaseLoader<GameMode>
    {
        private readonly DefinitionCache<MarketDefinition> marketCache;
        private readonly DefinitionCache<MaterialDefinition> materialCache;
        private readonly DefinitionCache<TransportDefinition> transportCache;
        private readonly DefinitionCache<MiningToolDefinition> miningToolCache;

        public GameModeLoader(DefinitionCache<GameMode> targetCache, DefinitionCache<MaterialDefinition> materialCache, DefinitionCache<MiningToolDefinition> miningToolCache, DefinitionCache<TransportDefinition> transportCache, DefinitionCache<MarketDefinition> marketCache) : base(targetCache)
        {
            this.marketCache = marketCache;
            this.materialCache = materialCache;
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
                    };

                    CheckItems(loadedGameMode.AvailableMiningTools, newGameMode.AvailableMiningTools, miningToolCache);
                    CheckItems(loadedGameMode.AvailableTransports, newGameMode.AvailableTransports, transportCache);

                    if (loadedGameMode.World != default)
                    {
                        newGameMode.World = new WorldDefinition()
                        {
                            Seed = loadedGameMode.World.Seed,
                            MaxWidth = loadedGameMode.World.MaxWidth,
                            Headquarters = loadedGameMode.World.Headquarters,
                        };

                        CheckItems(loadedGameMode.World.Materials, newGameMode.World.Materials, this.materialCache);
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
                        newGameMode.Inventory = new InventoryDefinition();

                        CheckItems(loadedGameMode.Inventory.Tools, newGameMode.Inventory.Tools, this.miningToolCache);
                        CheckItems(loadedGameMode.Inventory.HorizontalTransports, newGameMode.Inventory.HorizontalTransports, this.transportCache);
                        CheckItems(loadedGameMode.Inventory.VerticalTransports, newGameMode.Inventory.VerticalTransports, this.transportCache);
                    }

                    targetCache[loadedGameMode.Reference] = newGameMode;
                }
            }
        }
    }
}
