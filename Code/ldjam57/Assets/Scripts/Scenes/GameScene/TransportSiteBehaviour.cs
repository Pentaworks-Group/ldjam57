using Assets.Scripts.Core.Model;
using GameFrame.Core.Math;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

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
            var vect = worldBehaviour.GetUnityVector(shaft.GetPosition(), transform.position.z);
            transform.position = vect;
        }


        public void OnClicked()
        {
            BuildSite();
        }

        private TransportBehaviour GetTransportBehaviour() {
            var transB = worldBehaviour.GenerateTransportBehaviour(shaft, transport, direction);
            shaft.SetTransport(transB);
            return transB;
        }

        private void BuildSite()
        {
            GetTransportBehaviour();
            worldBehaviour.BuildTransporteSite(this);
        }

    }
}
