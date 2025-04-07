using Assets.Scripts.Core.Model;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts.Scenes.GameScene
{
    public class TransportSiteBehaviour : MonoBehaviour, IClickable
    {
        private WorldBehaviour worldBehaviour;
        private Transport transport;
        private ShaftBehaviour shaft;
        private Direction direction;


        public void Init(WorldBehaviour worldBehaviour, Transport transport, ShaftBehaviour shaft,  Direction direction)
        {
            this.worldBehaviour = worldBehaviour;
            this.transport = transport;
            this.shaft = shaft;
            this.direction = direction;
            UpdatePosition();

        }

       public void UpdatePosition()
        {
            if (direction == Direction.Right)
            {
                var lScale = transform.localScale;
                var scale = new Vector3(lScale.x, lScale.y / 2, lScale.z);
                var position = worldBehaviour.GetUnityVector(shaft.GetPosition(), transform.position.z, xOffset: (-(1 - scale.y ) / 2) * .9f);
                var rotation = transform.rotation;
                rotation *= Quaternion.Euler(0, 0, -90);
                transform.rotation = rotation;
                transform.position = position;
                transform.localScale = scale;
            }
            else if (direction == Direction.Left)
            {
                var lScale = transform.localScale;
                var scale = new Vector3(lScale.x, lScale.y / 2, lScale.z);

                var position = worldBehaviour.GetUnityVector(shaft.GetPosition(), transform.position.z, xOffset: ((1 - scale.y) / 2) * .9f);
                var rotation = transform.rotation;
                rotation *= Quaternion.Euler(0, 0, 90);
                transform.rotation = rotation;
                transform.position = position;
                transform.localScale = scale;
            }
            else
            {
                var lScale = transform.localScale;
                var scale = new Vector3(lScale.x, lScale.y / 2, lScale.z);

                var position = worldBehaviour.GetUnityVector(shaft.GetPosition(), transform.position.z, yOffset: ((1 - scale.y) / 2) * .9f);

                transform.position = position;
                transform.localScale = scale;

            }
                
        }

        public bool IsVertical()
        {
            return direction == Direction.Down;
        }


        public void OnClicked()
        {
            BuildSite();
        }

        private TransportBehaviour GetTransportBehaviour() {
            var transB = worldBehaviour.GenerateTransportBehaviour(shaft, transport, direction);
            shaft.TransportBehaviour  = transB;
            return transB;
        }

        private void BuildSite()
        {
            GetTransportBehaviour();
            worldBehaviour.BuildTransporteSite(this);
        }
    }
}
