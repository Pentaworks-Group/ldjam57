using Assets.Scripts.Core.Model;
using GameFrame.Core.Math;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class TransportSiteBehaviour : MonoBehaviour, IClickable
    {
        private WorldBehaviour worldBehaviour;
        private Transport transport;
        private Point2 position;
        private List<ShaftBehaviour> points = new ();


        public void Init(WorldBehaviour worldBehaviour, Transport transport, List<ShaftBehaviour> points, Point2 position)
        {
            this.worldBehaviour = worldBehaviour;
            this.transport = transport;
            this.points = points;
            this.position = position;
            UpdatePosition();
        }

        public void UpdatePosition()
        {
            var vect = worldBehaviour.GetUnityVector(position, transform.position.z);
            transform.position = vect;
        }


        public void OnClicked()
        {

        }

    }
}
