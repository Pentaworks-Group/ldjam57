using Assets.Scripts.Core;
using Assets.Scripts.Core.Model;
using Assets.Scripts.Core.Model.Inventories;
using GameFrame.Core.Math;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

namespace Assets.Scripts.Scenes.GameScene
{
    public class TransportSiteBuilder : Builder
    {
        private WorldBehaviour worldBehaviour;

        private TransportInventoryItem transportInventoryItem => (TransportInventoryItem)inventoryItem;


        public TransportSiteBuilder(WorldBehaviour worldBehaviour, InventoryItem inventoryItem)
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
            var possiblePositions = new List<(Point2, Direction)>();
            if (transportInventoryItem.IsVertical)
            {
                GetVerticalTransporterBuildSites(possiblePositions);
            }
            else
            {
                GetHorizontalTransporterBuildSites(possiblePositions);
            }

            return possiblePositions;
        }

        private void GetVerticalTransporterBuildSites(List<(Point2, Direction)> possiblePositions)
        {
            foreach (var shaft in worldBehaviour.Shafts)
            {
                if (shaft.HasTransport())
                {
                    continue;
                }
                possiblePositions.Add((shaft.GetPosition(), Direction.Down));
            }
        }
        private void GetHorizontalTransporterBuildSites(List<(Point2, Direction)> possiblePositions)
        {
            foreach (var shaft in worldBehaviour.Shafts)
            {
                if (shaft.HasTransport())
                {
                    continue;
                }
                possiblePositions.Add((shaft.GetPosition(), Direction.Left));
                possiblePositions.Add((shaft.GetPosition(), Direction.Right));
            }
        }
    }
}
