using Assets.Scripts.Core.Model;
using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class SiteBehaviour : TileBehaviour, IClickable
    {
        private MiningTool miningTool;
        private Direction direction;

        private void Awake()
        {
            this.digable = false;
        }

        public void Init(WorldBehaviour worldBehaviour, int pos, MiningTool miningTool, Direction direction)
        {
            base.Init(worldBehaviour, pos);
            this.miningTool = miningTool;
            this.direction = direction;
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

    }
}
