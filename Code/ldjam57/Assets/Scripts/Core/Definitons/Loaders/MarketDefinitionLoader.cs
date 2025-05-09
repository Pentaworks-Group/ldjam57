﻿using System;
using System.Collections.Generic;

using GameFrame.Core.Definitions.Loaders;

namespace Assets.Scripts.Core.Definitons.Loaders
{
    public class MarketDefinitionLoader : BaseLoader<MarketDefinition>
    {
        private readonly DefinitionCache<MineralDefinition> mineralCache;

        public MarketDefinitionLoader(DefinitionCache<MarketDefinition> targetCache, DefinitionCache<MineralDefinition> mineralCache) : base(targetCache)
        {
            this.mineralCache = mineralCache;
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
                        MineralValues = new List<MineralValueDefinition>(),
                        UpdateInterval = loadedMarketDefinition.UpdateInterval,
                        Volatility = loadedMarketDefinition.Volatility,
                        EnableRandomEvents = loadedMarketDefinition.EnableRandomEvents,
                        EventProbability = loadedMarketDefinition.EventProbability,
                        EventImpactMultiplier = loadedMarketDefinition.EventImpactMultiplier,
                        Events = new List<MarketEventDefinition>()
                    };

                    if (loadedMarketDefinition.MineralValues?.Count > 0)
                    {
                        foreach (var marketValue in loadedMarketDefinition.MineralValues)
                        {
                            var newMineralValueDefinition = new MineralValueDefinition()
                            {
                                Mineral = CheckItem(marketValue.Mineral, this.mineralCache),
                                Value = marketValue.Value,
                                Volatility = marketValue.Volatility,
                                MinPrice = marketValue.MinPrice,
                                MaxPrice = marketValue.MaxPrice,
                                TrendStrength = marketValue.TrendStrength,
                            };

                            marketDefinition.MineralValues.Add(newMineralValueDefinition);
                        }
                    }

                    if (loadedMarketDefinition.Events?.Count > 0)
                    {
                        foreach (var ev in loadedMarketDefinition.Events)
                        {
                            var newMarketEventDefinition = new MarketEventDefinition()
                            {
                                Name = ev.Name,
                                Description = ev.Description,
                                AffectedMaterials = ev.AffectedMaterials,
                                PriceImpact = ev.PriceImpact,
                                Duration = ev.Duration,
                            };

                            marketDefinition.Events.Add(newMarketEventDefinition);
                        }
                    }

                    targetCache[loadedMarketDefinition.Reference] = marketDefinition;
                }
            }
        }
    }
}
