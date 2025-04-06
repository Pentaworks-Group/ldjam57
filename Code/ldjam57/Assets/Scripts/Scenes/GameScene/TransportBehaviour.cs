using Assets.Scripts.Core.Model;
using GameFrame.Core.Math;
using NUnit.Framework;
using System.Collections.Generic;

namespace Assets.Scripts.Scenes.GameScene
{
    public class TransportBehaviour : IClickable
    {
        private WorldBehaviour worldBehaviour;
        private Transport transport;
        private Point2 startPos;
        private Point2 endPos;
        private List<Point2> points = new List<Point2>();


        public void Init(WorldBehaviour worldBehaviour, Transport transport)
        {
            this.worldBehaviour = worldBehaviour;
            this.transport = transport;
        }

        public void AddPoint(Point2 point)
        {
            points.Add(point);
        }


        public void OnClicked()
        {

        }

    }
}
