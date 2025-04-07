using Assets.Scripts.Core.Model;
using Assets.Scripts.Core.Model.Inventories;

using GameFrame.Core.Math;

namespace Assets.Scripts.Scenes.GameScene
{
    public class SiteBehaviour : TileBehaviour, IClickable
    {
        private MiningToolInventoryItem miningToolInventoryItem;
        private Direction direction;
        private Point2 pos;

        private void Awake()
        {
            this.digable = false;
        }

        public void Init(WorldBehaviour worldBehaviour, Point2 pos, MiningToolInventoryItem miningToolInventoryItem, Direction direction)
        {
            base.Init(worldBehaviour);
            this.miningToolInventoryItem = miningToolInventoryItem;
            this.direction = direction;
            this.pos = pos;
        }

        public void OnClicked()
        {
            miningToolInventoryItem.Amount -= 1;
            worldBehaviour.BuildDigSite(this);
        }

        public MiningToolInventoryItem GetMiningTool()
        {
            return miningToolInventoryItem;
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
