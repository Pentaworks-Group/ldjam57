using Assets.Scripts.Core.Model;
using GameFrame.Core.Math;
using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class SiteBehaviour : TileBehaviour, IClickable
    {
        private MiningTool miningTool;
        private Direction direction;
        private Point2 pos;

        private void Awake()
        {
            this.digable = false;
        }

        public void Init(WorldBehaviour worldBehaviour, Point2 pos, MiningTool miningTool, Direction direction)
        {
            base.Init(worldBehaviour);
            this.miningTool = miningTool;
            this.direction = direction;
            this.pos = pos;
        }


        public void OnClicked()
        {
            worldBehaviour.BuildDigSite(this);
        }

        public MiningTool GetMiningTool()
        {
            return miningTool;
        }

        public Direction GetDirection()
        {
            return direction;
        }

        public override Point2 GetPosition()
        {
            return pos;
        }
    }
}
