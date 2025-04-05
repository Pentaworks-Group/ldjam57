using UnityEngine;
namespace Assets.Scripts.Scenes.GameScene
{
    public class TileBehaviour : MonoBehaviour
    {
        protected WorldBehaviour worldBehaviour;
        protected int pos;
        protected bool digable = true;
            

        public void Init(WorldBehaviour worldBehaviour, int pos)
        {
            this.worldBehaviour = worldBehaviour;
            this.pos = pos;
        }

        public int GetPosition() { return pos; }

        public bool IsDigable() { return digable; }

    }
}
