using GameFrame.Core.Math;
using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class ShaftBehaviour : TileBehaviour, IClickable
    {
        private Point2 pos;
        public TransporterBehaviour TransportBehaviour { get; set; }
        public DiggerBehaviour DiggerBehaviour { get; set; }

        private void Awake()
        {
            this.digable = false;
        }
              
        public void Init(WorldBehaviour worldBehaviour, Point2 pos)
        {
            base.Init(worldBehaviour);
            this.pos = pos;
        }

        public bool HasTransport()
        {
            return this.TransportBehaviour != null;
        }

        public void OnClicked()
        {

        }

        public override Point2 GetPosition()
        {
            return pos;
        }
    }
}
