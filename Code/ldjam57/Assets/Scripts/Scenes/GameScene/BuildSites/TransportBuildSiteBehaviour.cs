using Assets.Scripts.Core.Model;
using Assets.Scripts.Core.Model.Inventories;

using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Assets.Scripts.Scenes.GameScene
{
    public class TransportBuildSiteBehaviour : BuildSiteBehaviour
    {

        [SerializeField]
        private TransporterBehaviour TransporterTemplate;

        private TransportInventoryItem TransportInventoryItem => (TransportInventoryItem)inventoryItem;


        protected override void BuildSite()
        {
            var newTransporterBehaviour = GameObject.Instantiate(TransporterTemplate, buildSiteManagerBehaviour.tileParent.transform);

            var shaftBehaviour = buildSiteManagerBehaviour.worldBehaviour.GetTileRelative(position);
            newTransporterBehaviour.Init(buildSiteManagerBehaviour.worldBehaviour, (ShaftBehaviour)shaftBehaviour, TransportInventoryItem.Transport, direction);
            newTransporterBehaviour.gameObject.name = "Transport" + position.X + "," + position.Y;
            newTransporterBehaviour.gameObject.SetActive(true);

            buildSiteManagerBehaviour.RemoveBuildSite(this);
        }
    }
}
