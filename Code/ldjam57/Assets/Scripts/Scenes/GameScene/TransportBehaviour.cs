using GameFrame.Core.Math;
using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class TransportBehaviour : MonoBehaviour, IClickable
    {

        private TransportRoute transportRoute;
        private ShaftBehaviour shaftBehaviour;

        public void Init(ShaftBehaviour shaftBehaviour, Core.Model.Transport transport)
        {
            this.shaftBehaviour = shaftBehaviour;

            Renderer renderer = gameObject.GetComponent<Renderer>();
            var tt = GameFrame.Base.Resources.Manager.Sprites.Get(transport.Sprite);
            renderer.material.mainTexture = GameFrame.Base.Resources.Manager.Textures.Get(transport.Sprite);
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
