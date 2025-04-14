using Assets.Scripts.Core;
using Assets.Scripts.Core.Model;
using Assets.Scripts.Core.Model.Inventories;
using GameFrame.Core.Math;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class DigSiteBuilder : Builder
    {
        [SerializeField]
        private BuildSiteBehaviour digBuildSiteTemplate;

        private WorldBehaviour worldBehaviour;
        private BuildSiteManagerBehaviour buildSiteManagerBehaviour;

        private MiningToolInventoryItem MiningToolInventoryItem => (MiningToolInventoryItem)inventoryItem;


        public DigSiteBuilder(BuildSiteManagerBehaviour buildSiteManagerBehaviour, InventoryItem inventoryItem)
        {
            this.inventoryItem = inventoryItem;
            this.worldBehaviour = buildSiteManagerBehaviour.worldBehaviour;
            this.buildSiteManagerBehaviour = buildSiteManagerBehaviour;

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

        public override BuildSiteBehaviour GenerateBuildSite(Point2 pos, Direction dir)
        {

            var site = GameObject.Instantiate(digBuildSiteTemplate, buildSiteManagerBehaviour.siteParent.transform);
            site.Init(buildSiteManagerBehaviour, pos, inventoryItem, dir);
            return site;
        }
    }
}
