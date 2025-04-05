using GameFrame.Core.Math;
using UnityEngine;
namespace Assets.Scripts.Scenes.GameScene
{
    public class TileBehaviour : MonoBehaviour
    {
        protected WorldBehaviour worldBehaviour;
        protected Point2 pos;
        protected bool digable = true;
            

        public void Init(WorldBehaviour worldBehaviour, Point2 pos)
        {
            this.worldBehaviour = worldBehaviour;
            this.pos = pos;
        }

        public Point2 GetPosition() { return pos; }

        public bool IsDigable() { return digable; }

    }
}
