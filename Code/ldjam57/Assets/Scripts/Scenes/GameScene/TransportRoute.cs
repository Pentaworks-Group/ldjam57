
using Assets.Scripts.Core.Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.Scenes.GameScene
{
    public class TransportRoute
    {
        private WorldBehaviour worldBehaviour;
        private List<TransportBehaviour> points = new List<TransportBehaviour>();
        private Transport transport;
        private UnityEngine.Vector3 start;
        private UnityEngine.Vector3 stop;

        private bool vertical = false;

        public TransportRoute(WorldBehaviour worldBehaviour, bool vertical, Transport transport)
        {
            this.worldBehaviour = worldBehaviour;
            this.vertical = vertical;
            this.transport = transport;
        }

        public void SetPoints(List<TransportBehaviour> points)
        {
            this.points = points;
            SetStartStop();
        }

        public Texture GetTexture()
        {
            var texture = GameFrame.Base.Resources.Manager.Textures.Get(transport.Sprite);
            return texture;
        }

        public bool IsVertical()
        {
            return vertical;
        }

        private void SetStartStop()
        {

            if (vertical)
            {
                points = points.OrderBy(point => point.GetPosition().Y).ToList();
            }
            else
            {
                points = points.OrderBy(point => point.GetPosition().Y).ToList();
            }
            var firstItem = points[0];
            start = worldBehaviour.GetUnityVector(firstItem.GetPosition(), firstItem.transform.position.z);
            var lastItem = points.Last();
            stop = worldBehaviour.GetUnityVector(lastItem.GetPosition(), lastItem.transform.position.z);

        }

        public void OnClicked()
        {
        }


    }
}
