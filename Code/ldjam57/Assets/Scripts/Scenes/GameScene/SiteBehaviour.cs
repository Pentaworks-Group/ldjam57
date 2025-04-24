using Assets.Scripts.Core.Model;

using GameFrame.Core.Math;

namespace Assets.Scripts.Scenes.GameScene
{
    public class SiteBehaviour : TileBehaviour, IClickable
    {
        private MiningTool miningTool;
        private Direction direction;
        private Point2 position;

        private void Awake()
        {
            this.digable = false;
        }

        public void Init(WorldBehaviour worldBehaviour, Point2 pos, MiningTool miningTool, Direction direction)
        {
            base.Init(worldBehaviour);
            this.miningTool = miningTool;
            this.direction = direction;
            this.position = pos;
        }

        public void OnClicked()
        {
            //miningToolInventoryItem.Amount -= 1;

            var digger = new Digger()
            {
                Direction = direction,
                MiningTool = miningTool,
                Position = position
            };

            worldBehaviour.AddDigSite(digger);
        }

        public Direction GetDirection()
        {
            return direction;
        }

        public override Point2 GetPosition()
        {
            return position;
        }
    }
}
