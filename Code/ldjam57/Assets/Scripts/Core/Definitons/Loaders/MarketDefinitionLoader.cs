using System.Collections.Generic;

using GameFrame.Core.Definitions.Loaders;

namespace Assets.Scripts.Core.Definitons.Loaders
{
    public class MarketDefinitionLoader : BaseLoader<MarketDefinition>
    {
        private readonly DefinitionCache<MineralDefinition> materialCache;

        public MarketDefinitionLoader(DefinitionCache<MarketDefinition> targetCache, DefinitionCache<MineralDefinition> materialCache) : base(targetCache)
        {
            this.materialCache = materialCache;
        }

        protected override void OnDefinitionsLoaded(List<MarketDefinition> definitions)
        {
            if (definitions?.Count > 0)
            {
                foreach (var loadedMarketDefinition in definitions)
                {
                    var marketDefinition = new MarketDefinition()
                    {
                        Reference = loadedMarketDefinition.Reference,
                        Name = loadedMarketDefinition.Name,
                        Factor = loadedMarketDefinition.Factor,
                        MaterialValues = new List<MaterialValueDefinition>()
                    };

                    if (loadedMarketDefinition.MaterialValues?.Count > 0)
                    {
                        foreach (var marketValue in loadedMarketDefinition.MaterialValues)
                        {
                            var newMaterialValueDefinition = new MaterialValueDefinition()
                            {
                                Material = CheckItem(marketValue.Material, this.materialCache),
                                Value = marketValue.Value
                            };

                            marketDefinition.MaterialValues.Add(newMaterialValueDefinition);
                        }
                    }

                    targetCache[loadedMarketDefinition.Reference] = marketDefinition;
                }
            }
        }
    }
}
