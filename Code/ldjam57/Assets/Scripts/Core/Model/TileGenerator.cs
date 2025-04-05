using System;
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

        public Tile GenerateTile(Int32 x, Int32 y, List<Mineral> possibleMaterials)
        {
            var tile = new Tile()
            {
                Position = new GameFrame.Core.Math.Point2(x, y),
                MaterialAmounts = GenerateMineralAmounts(x, y, possibleMaterials)
            };

            return tile;
        }

        private Dictionary<String, Double> GenerateMineralAmounts(Int32 x, Int32 y, List<Mineral> possibleMaterials)
        {
            var mineralAmounts = new Dictionary<String, Double>();

            foreach (var mineral in world.Minerals)
            {
                mineralAmounts[mineral.Reference] = 0;
            }

            var possibleMaterialValues = new Dictionary<String, Double>();

            foreach (var possibleMaterial in possibleMaterials)
            {
                var materialX = x + possibleMaterial.Seed;
                var materialY = y + possibleMaterial.Seed;

                var perlinValue = UnityEngine.Mathf.PerlinNoise(x, y);
            }

            return mineralAmounts;
        }

        private List<Mineral> GetMatchingMinerals(Int32 y)
        {
            var materialList = new List<Mineral>();

            foreach (var material in world.Minerals)
            {
                if (material.SpawnRange == default || material.SpawnRange.Contains(y))
                {
                    materialList.Add(material);
                }
            }

            return materialList;
        }
    }
}
