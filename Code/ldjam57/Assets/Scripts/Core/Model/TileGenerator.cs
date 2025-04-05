using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            for (int i = 0; i < world.Width; i++)
            {
                tiles.Add(GenerateTile(i, 0));
                tiles.Add(GenerateTile(i, 1));
            }

            return tiles;
        }

        public Tile GenerateTile(Int32 x, Int32 y)
        {
            var tile = new Tile()
            {
                Position = new GameFrame.Core.Math.Vector2(x, y),
                MaterialAmounts = GenerateMaterialAmounts(x, y)
            };

            return tile;
        }

        private Dictionary<String, Double> GenerateMaterialAmounts(Int32 x, Int32 y)
        {
            var materialAmounts = new Dictionary<String, Double>();

            var matchingMaterials = GetMatchingMaterials(y);

            if (matchingMaterials.Count == 1)
            {
                var material = matchingMaterials[0];

                materialAmounts[material.Reference] = 1; // Amount To be calculated or taken from NoiseMap
            }
            else if (matchingMaterials.Count > 1)
            {
                foreach (var material in matchingMaterials)
                {
                    materialAmounts[material.Reference] = 1; // Amount To be calculated or taken from NoiseMap
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(matchingMaterials), "For some reason, no matching Material was found");
            }

            return materialAmounts;
        }

        private List<Assets.Scripts.Core.Definitons.MaterialDefinition> GetMatchingMaterials(Int32 y)
        {
            var materialList = new List<Definitons.MaterialDefinition>();

            foreach (var material in world.Definition.Materials)
            {
                if (material.SpawnRange !=default)
                {                    
                    if (material.SpawnRange.Contains(y))
                    {
                        materialList.Add(material);
                    }
                }
                else
                {
                    materialList.Add(material);
                }
            }

            return materialList;
        }
    }
}
