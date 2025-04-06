using Assets.Scripts.Core.Model;
using GameFrame.Core.Math;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using UnityEngine.UIElements;
using UnityEngine.WSA;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts.Scenes.GameScene
{
    public class TransportSiteBehaviour : MonoBehaviour, IClickable
    {
        private WorldBehaviour worldBehaviour;
        private Transport transport;
        private ShaftBehaviour shaft;
        private bool vertical;
        private Direction direction;


        public void Init(WorldBehaviour worldBehaviour, Transport transport, ShaftBehaviour shaft, bool vertical, Direction direction)
        {
            this.worldBehaviour = worldBehaviour;
            this.transport = transport;
            this.shaft = shaft;
            this.vertical = vertical;
            this.direction = direction;
            UpdatePosition();
        }

        public void UpdatePosition()
        {
            if (direction == Direction.Left)
            {
                var lScale = transform.localScale;
                var scale = new Vector3(lScale.x, lScale.y / 2, lScale.z);
                var position = worldBehaviour.GetUnityVector(shaft.GetPosition(), transform.position.z, xOffset: -(1 - scale.y ) / 2);
                var rotation = transform.rotation;
                rotation *= Quaternion.Euler(0, 0, -90);
                transform.rotation = rotation;
                transform.position = position;
                transform.localScale = scale;
            }
            else if (direction == Direction.Right)
            {
                var lScale = transform.localScale;
                var scale = new Vector3(lScale.x, lScale.y / 2, lScale.z);

                var position = worldBehaviour.GetUnityVector(shaft.GetPosition(), transform.position.z, xOffset: (1 - scale.y) / 2);
                var rotation = transform.rotation;
                rotation *= Quaternion.Euler(0, 0, 90);
                transform.rotation = rotation;
                transform.position = position;
                transform.localScale = scale;
            }
            else
            {
                var vect = worldBehaviour.GetUnityVector(shaft.GetPosition(), transform.position.z);
                transform.position = vect;
            }
                
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
