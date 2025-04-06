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
        private List<Point2> points = new List<Point2>();


        public void Init(WorldBehaviour worldBehaviour, Transport transport, List<Point2> points, Point2 position)
        {
            this.worldBehaviour = worldBehaviour;
            this.transport = transport;
            this.points = points;
            this.position = position;
        }

        public void 


        public void OnClicked()
        {

        }

    }
}
