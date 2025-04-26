using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Core.Model
{
    public class TileGenerator
    {
        private readonly World world;

        public TileGenerator(World world)
        {
            this.world = world;
        }

        public Map<Int32, Tile> TileMap { get; }

        public List<Tile> GenerateRootTiles(Int32 initialDepth)
        {
            var tiles = new List<Tile>();

            for (int y = 0; y < initialDepth; y++)
            {
                var possibleMinerals = GetMatchingMinerals(y);

                for (int i = 0; i < world.Width; i++)
                {
                    GenerateTile(i, y, possibleMinerals);
                }
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

            this.world.Tiles.Add(tile);
             
            return tile;
        }

        private List<MineralAmount> GenerateMineralAmounts(Int32 x, Int32 y, List<Mineral> possibleMinerals)
        {
            var mineralAmounts = new Dictionary<String, MineralAmount>();

            foreach (var mineral in world.Minerals)
            {
                mineralAmounts[mineral.Reference] = new MineralAmount(mineral);
            }

            var possibleMineralValues = new Dictionary<String, Double>();

            var total = 0d;

            //var densityscaling = 0.69f;
            //var cutoffscaling = 0.35;
            var cutoffscaling = 0.5;

            foreach (var possibleMineral in possibleMinerals)
            {
                var mineralX = (x + world.Seed + possibleMineral.Seed);
                var mineralY = (y + world.Seed + possibleMineral.Seed);

                //var perlinValue = UnityEngine.Mathf.PerlinNoise(mineralX, mineralY) * densityscaling;
                var perlinValue = UnityEngine.Mathf.PerlinNoise(mineralX, mineralY);

                if (perlinValue > cutoffscaling)
                {
                    possibleMineralValues[possibleMineral.Reference] = perlinValue;

                    total += perlinValue;
                }
            }

            if (total > 1)
            {
                var scale = 1.0d / total;

                foreach (var mineralKeyValuePair in possibleMineralValues)
                {
                    mineralAmounts[mineralKeyValuePair.Key].Amount = mineralKeyValuePair.Value * scale;
                }
            }
            else
            {
                foreach (var layer in possibleMineralValues)
                {
                    mineralAmounts[layer.Key].Amount = layer.Value;
                }

                mineralAmounts[world.DefaultMineral.Reference].Amount = 1 - total;
            }

            return mineralAmounts.Values.ToList();
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
