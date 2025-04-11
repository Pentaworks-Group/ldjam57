using Assets.Scripts.Core.Model;
using Assets.Scripts.Core.Model.Inventories;
using GameFrame.Core.Math;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class BuildSiteManagerBehaviour : MonoBehaviour
    {
        [SerializeField]
        private BuildSiteBehaviour digBuildSiteTemplate;
        [SerializeField]
        private BuildSiteBehaviour transportBuildSiteTemplate;
        public GameObject tileParent;
        [SerializeField]
        public WorldBehaviour worldBehaviour;



        private List<BuildSiteBehaviour> buildSites = new();
        private Dictionary<InventoryItem, Builder> builders = new();

        public Builder SelectedBuilder { get; private set; }


        public void SelectTool(InventoryItem inventoryItem)
        {
            if (inventoryItem == null) return;
            if (!builders.TryGetValue(inventoryItem, out Builder builder)) {
                if (inventoryItem is MiningToolInventoryItem)
                {
                    builder = new DigSiteBuilder(worldBehaviour, inventoryItem);
                    builders.Add(inventoryItem, builder);
                }
                else if (inventoryItem is TransportInventoryItem)
                {
                    builder = new TransportSiteBuilder(worldBehaviour, inventoryItem);
                    builders.Add(inventoryItem, builder);
                }
            }
            SelectBuilder(builder);
        }


        public bool IsBuilderSelected(Builder builder)
        {
            return SelectedBuilder == builder;
        }

        public void SelectBuilder(Builder builder)
        {
            if (builder == null)
            {
                return;
            }
            ClearDigSites();
            SelectedBuilder = builder;
            CreateBuildSites();
        }

        public void CreateBuildSites()
        {
            var possibleSites = SelectedBuilder.GetPossibleBuildSites();
            buildSites = new List<BuildSiteBehaviour>();

            if (SelectedBuilder.inventoryItem is TransportInventoryItem transportInventoryItem)
            {
                buildSites = possibleSites.Select(pS => GenerateTransportBuildSite(pS.Item1, pS.Item2)).ToList();
            }
            else
            {
                buildSites = possibleSites.Select(pS => GenerateDigBuildSite(pS.Item1, pS.Item2)).ToList();
            }
            buildSites.ForEach(site => site.gameObject.SetActive(true));
        }

        private BuildSiteBehaviour GenerateDigBuildSite(Point2 pos, Direction dir)
        {

            var site = GameObject.Instantiate(digBuildSiteTemplate, tileParent.transform);
            site.Init(this, pos, SelectedBuilder.inventoryItem, dir);
            return site;
        }

        private BuildSiteBehaviour GenerateTransportBuildSite(Point2 pos, Direction dir)
        {

            var site = GameObject.Instantiate(transportBuildSiteTemplate, tileParent.transform);
            site.Init(this, pos, SelectedBuilder.inventoryItem, dir);
            return site;
        }

        public void RemoveBuildSite(BuildSiteBehaviour site)
        {
            buildSites.Remove(site);
        }

        public void ClearDigSites()
        {
            foreach (var site in buildSites)
            {
                GameObject.Destroy(site.gameObject);
            }

            buildSites.Clear();
        }

    }
}
