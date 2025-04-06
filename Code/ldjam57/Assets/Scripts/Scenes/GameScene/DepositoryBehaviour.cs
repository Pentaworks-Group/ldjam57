using Assets.Scripts.Core.Model;

using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class DepositoryBehaviour : MonoBehaviour, IClickable
    {
        private WorldBehaviour worldBehaviour;
        private Depository depository;

        public void Init(WorldBehaviour worldBehaviour, Depository depository)
        {
            this.worldBehaviour = worldBehaviour;
            this.depository = depository;
        }

        public void OnClicked()
        {

        }
    }
}
