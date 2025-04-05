
using Assets.Scripts.Core.Definitons;
using Assets.Scripts.Core.Model;
using GameFrame.Core.Math;

namespace Assets.Scripts.Scenes.GameScene
{
    public class GroundBehaviour : TileBehaviour
    {
        private MineralDefinition mineral;
        private double miningProgress = 0;

        public void Init(WorldBehaviour worldBehaviour, Point2 pos, MineralDefinition mineral)
        {
            base.Init(worldBehaviour, pos);
            this.mineral = mineral;
        }

        public bool UpdateProgress(MiningTool miningTool)
        {
            var progress = miningTool.SpeedFactor / mineral.MiningSpeedFactor.GetValueOrDefault(1);
            miningProgress += progress;
            var finished = IsMined();
            if (finished)
            {
                worldBehaviour.ReplaceTile(this);
            }
            return finished;
        }

        public bool IsMined()
        {
            return miningProgress >= 1;
        }

    }
}
