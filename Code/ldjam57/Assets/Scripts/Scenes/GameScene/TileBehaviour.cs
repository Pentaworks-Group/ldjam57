using GameFrame.Core.Math;
using UnityEngine;
namespace Assets.Scripts.Scenes.GameScene
{
    public abstract class TileBehaviour : MonoBehaviour
    {
        protected WorldBehaviour worldBehaviour;
        protected bool digable = true;
            

        public void Init(WorldBehaviour worldBehaviour)
        {
            this.worldBehaviour = worldBehaviour;
        }

        public abstract Point2 GetPosition();

        public bool IsDigable() { return digable; }

    }
}
