
using Assets.Scripts.Core.Definitons;
using Assets.Scripts.Core.Model;
using GameFrame.Core.Math;

namespace Assets.Scripts.Scenes.GameScene
{
    public class GroundBehaviour : TileBehaviour
    {
        private Tile tile;

        public void Init(WorldBehaviour worldBehaviour, Tile tile)
        {
            base.Init(worldBehaviour);
            this.tile = tile;
        }

        public bool UpdateProgress(MiningTool miningTool)
        {
            float progress = (float)(miningTool.SpeedFactor / tile.SpeedFactor);
            tile.DigingProgress += progress;
            var finished = IsMined();
            if (finished)
            {
                worldBehaviour.ReplaceTile(this);
            }
            return finished;
        }

        public bool IsMined()
        {
            return tile.DigingProgress >= 1;
        }

        public override Point2 GetPosition()
        {
            return tile.Position;
        }
    }
}
