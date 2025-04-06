using GameFrame.Core.Math;
using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class TransportBehaviour : MonoBehaviour, IClickable
    {

        private TransportRoute transportRoute;
        private ShaftBehaviour shaftBehaviour;

        public void Init(TransportRoute transportRoute, ShaftBehaviour shaftBehaviour)
        {
            this.transportRoute = transportRoute;
            this.shaftBehaviour = shaftBehaviour;

            Renderer renderer = gameObject.GetComponent<Renderer>();

            renderer.material.mainTexture = transportRoute.GetTexture();
        }

        public void OnClicked()
        {
            transportRoute.OnClicked();
        }

        public Point2 GetPosition()
        {
            return shaftBehaviour.GetPosition();
        }

    }
}
