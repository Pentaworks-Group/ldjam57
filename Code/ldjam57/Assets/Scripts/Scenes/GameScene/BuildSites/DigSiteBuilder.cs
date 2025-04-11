using Assets.Scripts.Core;
using Assets.Scripts.Core.Model;
using Assets.Scripts.Core.Model.Inventories;
using GameFrame.Core.Math;
using System.Collections.Generic;

namespace Assets.Scripts.Scenes.GameScene
{
    public class DigSiteBuilder : Builder
    {
        private WorldBehaviour worldBehaviour;

        private MiningToolInventoryItem MiningToolInventoryItem => (MiningToolInventoryItem)inventoryItem;


        public DigSiteBuilder(WorldBehaviour worldBehaviour, InventoryItem inventoryItem)
        {
            this.inventoryItem = inventoryItem;
            this.worldBehaviour = worldBehaviour;

        }

        public override List<Direction> GetPossiblBuildDirections(Point2 position)
        {
            throw new System.NotImplementedException();
        }

        public override List<(Point2, Direction)> GetPossibleBuildSites()
        {
            var possiblePositions = new List<(Point2, Direction)> ();
            Map<int, Digger> diggerMap = GenerateDiggerMap();

            foreach (var shaft in worldBehaviour.Shafts)
            {
                var beneath = worldBehaviour.GetTileRelative(shaft.GetPosition(), 0, 1);

                if (beneath?.IsDigable() == true && !diggerMap.TryGetValue(shaft.GetPosition().X, shaft.GetPosition().Y, out _))
                {
                    if (MiningToolInventoryItem.MiningTool.Size.X < 2)
                    {
                        var tile = worldBehaviour.GetTileRelative(shaft.GetPosition(), -1, 0);
                        if (tile?.IsDigable() == true)
                        {
                            possiblePositions.Add((shaft.GetPosition(),Direction.Left));
                        }
                        tile = worldBehaviour.GetTileRelative(shaft.GetPosition(), 1, 0);
                        if (tile?.IsDigable() == true)
                        {
                            possiblePositions.Add((shaft.GetPosition(), Direction.Right));
                        }
                        tile = worldBehaviour.GetTileRelative(shaft.GetPosition(), 0, 1);
                        if (tile?.IsDigable() == true)
                        {
                            possiblePositions.Add((shaft.GetPosition(), Direction.Down));
                        }
                    }
                }
            }
            return possiblePositions;
        }


        public Map<int, Digger> GenerateDiggerMap()
        {
            Map<int, Digger> diggerMap = new();

            foreach (var digger in Base.Core.Game.State.ActiveDiggers)
            {
                diggerMap[digger.Position.X, digger.Position.Y] = digger;
            }

            return diggerMap;
        }
    }
}
