﻿using System;
using System.Collections.Generic;

namespace Assets.Scripts.Core.Model
{
    public class TileGenerator
    {
        private readonly World world;

        public TileGenerator(World world)
        {
            this.world = world;
        }

        public List<Tile> GenerateRootTiles()
        {
            var tiles = new List<Tile>();

            var possibleMineralsLevel0 = GetMatchingMinerals(0);
            var possibleMineralsLevel1 = GetMatchingMinerals(1);

            for (int i = 0; i < world.Width; i++)
            {
                tiles.Add(GenerateTile(i, 0, possibleMineralsLevel0));
                tiles.Add(GenerateTile(i, 1, possibleMineralsLevel1));
            }

            return tiles;
        }

        public Tile GenerateTile(Int32 x, Int32 y)
        {
            var possibleMinerals = GetMatchingMinerals(y);

            return GenerateTile(x, y, possibleMinerals);
        }

        private Tile GenerateTile(Int32 x, Int32 y, List<Mineral> possibleMinerals)
        {
            var tile = new Tile()
            {
                Position = new GameFrame.Core.Math.Point2(x, y),
                MineralAmounts = GenerateMineralAmounts(x, y, possibleMinerals)
            };

            return tile;
        }

        private Dictionary<String, Double> GenerateMineralAmounts(Int32 x, Int32 y, List<Mineral> possibleMinerals)
        {
            var mineralAmounts = new Dictionary<String, Double>();

            foreach (var mineral in world.Minerals)
            {
                mineralAmounts[mineral.Reference] = 0;
            }

            var possibleMineralValues = new Dictionary<String, Double>();

            var minValue = Single.MaxValue;
            var maxValue = Single.MinValue;

            var total = 0d;

            foreach (var possibleMineral in possibleMinerals)
            {
                var mineralX = (x + world.Seed + possibleMineral.Seed);
                var mineralY = (y + world.Seed + possibleMineral.Seed);

                var perlinValue = UnityEngine.Mathf.PerlinNoise(mineralX, mineralY);

                UnityEngine.Debug.Log(String.Format("{0};{1} - {2}", x, y, perlinValue));

                if (perlinValue > 0.5)
                {
                    possibleMineralValues[possibleMineral.Reference] = perlinValue;

                    if (perlinValue > maxValue)
                    {
                        maxValue = perlinValue;
                    }

                    if (perlinValue < minValue)
                    {
                        minValue = perlinValue;
                    }

                    total += perlinValue;
                }
            }

            if (total > 1)
            {
                var scale = 1.0d / total;

                foreach (var layer in possibleMineralValues)
                {
                    mineralAmounts[layer.Key] = layer.Value * scale;
                }
            }
            else
            {
                foreach (var layer in possibleMineralValues)
                {
                    mineralAmounts[layer.Key] = layer.Value;
                }

                mineralAmounts[world.DefaultMineral.Reference] = 1 - total;
            }

            return mineralAmounts;
        }

        private List<Mineral> GetMatchingMinerals(Int32 y)
        {
            var mineralList = new List<Mineral>();

            foreach (var mineral in world.Minerals)
            {
                if (!mineral.IsDefault)
                {
                    if (mineral.SpawnRange == default || mineral.SpawnRange.Contains(y))
                    {
                        mineralList.Add(mineral);
                    }
                }
            }

            return mineralList;
        }
    }
}
