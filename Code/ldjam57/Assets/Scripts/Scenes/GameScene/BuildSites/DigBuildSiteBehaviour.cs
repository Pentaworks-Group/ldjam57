using Assets.Scripts.Core.Model;
using Assets.Scripts.Core.Model.Inventories;

using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class DigBuildSiteBehaviour : BuildSiteBehaviour
    {

        [SerializeField]
        private DiggerBehaviour DiggerTemplate;

        private MiningToolInventoryItem MiningToolInventoryItem => (MiningToolInventoryItem)inventoryItem;


        protected override void BuildSite()
        {
            var newDigger = GameObject.Instantiate(DiggerTemplate, buildSiteManagerBehaviour.tileParent.transform);

            var digger = new Digger()
            {
                Direction = direction,
                MiningTool = MiningToolInventoryItem.MiningTool,
                Position = position
            };

            newDigger.Init(buildSiteManagerBehaviour.worldBehaviour, digger);
            newDigger.UpdatePosition();
            newDigger.gameObject.SetActive(true);
            Base.Core.Game.State.ActiveDiggers.Add(digger);

            buildSiteManagerBehaviour.RemoveBuildSite(this);
        }
    }
}
