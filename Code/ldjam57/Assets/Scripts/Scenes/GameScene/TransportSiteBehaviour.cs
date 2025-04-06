using Assets.Scripts.Core.Model;
using GameFrame.Core.Math;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class TransportSiteBehaviour : MonoBehaviour, IClickable
    {
        private WorldBehaviour worldBehaviour;
        private Transport transport;
        private Point2 position;
        private List<ShaftBehaviour> points = new ();
        private bool vertical;


        public void Init(WorldBehaviour worldBehaviour, Transport transport, List<ShaftBehaviour> points, Point2 position, bool vertical)
        {
            this.worldBehaviour = worldBehaviour;
            this.transport = transport;
            this.points = points;
            this.position = position;
            this.vertical = vertical;
            UpdatePosition();
        }

        public void UpdatePosition()
        {
            var vect = worldBehaviour.GetUnityVector(position, transform.position.z);
            transform.position = vect;
        }


        public void OnClicked()
        {
            BuildSite();
        }

        private TransportBehaviour GetTransportBehaviour(ShaftBehaviour shaft, TransportRoute transportRoute) {
            var transB = worldBehaviour.GenerateTransportBehaviour(transportRoute, shaft);
            return transB;
        }

        private void BuildSite()
        {
            var transportRoute = new TransportRoute(worldBehaviour, vertical, transport);
            var trans = points.Select(point => GetTransportBehaviour(point, transportRoute)).ToList();
            transportRoute.SetPoints(trans);
        }

    }
}
