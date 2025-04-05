
using Assets.Scripts.Core.Definitons;
using Assets.Scripts.Core.Model;

namespace Assets.Scripts.Scenes.GameScene
{
    public class GroundBehaviour : TileBehaviour
    {
        private MaterialDefinition material;
        private double miningProgress = 0;

        public void Init(WorldBehaviour worldBehaviour, int pos, MaterialDefinition material)
        {
            base.Init(worldBehaviour, pos);
            this.material = material;
        }

        public bool UpdateProgress(MiningTool miningTool)
        {
            var progress = miningTool.SpeedFactor / material.MiningSpeedFactor.GetValueOrDefault(1);
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
