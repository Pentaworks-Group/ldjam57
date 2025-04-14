using Assets.Scripts.Core;
using Assets.Scripts.Core.Model;
using Assets.Scripts.Core.Model.Inventories;
using GameFrame.Core.Math;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class TransportSiteBuilder : Builder
    {
        [SerializeField]
        private BuildSiteBehaviour transportBuildSiteTemplate;
        private WorldBehaviour worldBehaviour;
        private BuildSiteManagerBehaviour buildSiteManagerBehaviour;

        private TransportInventoryItem transportInventoryItem => (TransportInventoryItem)inventoryItem;


        public TransportSiteBuilder(BuildSiteManagerBehaviour buildSiteManagerBehaviour, InventoryItem inventoryItem)
        {
            this.inventoryItem = inventoryItem;
            this.worldBehaviour = buildSiteManagerBehaviour.worldBehaviour;
            this.buildSiteManagerBehaviour = buildSiteManagerBehaviour;
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

        public override BuildSiteBehaviour GenerateBuildSite(Point2 pos, Direction dir)
        {

            var site = GameObject.Instantiate(transportBuildSiteTemplate, buildSiteManagerBehaviour.siteParent.transform);
            site.Init(buildSiteManagerBehaviour, pos, inventoryItem, dir);
            return site;
        }
    }
}
