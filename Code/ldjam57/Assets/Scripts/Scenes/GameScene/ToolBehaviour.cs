using Assets.Scripts.Core.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class ToolBehaviour : TileBehaviour, IClickable
    {
        private MiningTool miningTool;
        private Direction direction;
        private List<GroundBehaviour> targets = new List<GroundBehaviour>();

        private bool isMining = false;
        private float tickInterval = 1f;
        private float tick = 0f;



        private void Awake()
        {
            this.digable = false;
            tick = tickInterval;
        }

        private void Update()
        {
            if (isMining) {
                tick -= Time.deltaTime;
                if (tick < 0f)
                {
                    MineTargets();
                    tick = tickInterval;
                }
            }
        }

        public void Init(WorldBehaviour worldBehaviour, MiningTool minigTool, Direction direction, int pos)
        {
            base.Init(worldBehaviour, pos);
            this.miningTool = minigTool;
            this.direction = direction;
            StartMining();
        }

        private void MineTargets()
        {
            for (int i = targets.Count - 1; i >= 0; i--)
            {
                if (targets[i].UpdateProgress(miningTool))
                {
                    targets.RemoveAt(i);
                }
            }
            if (targets.Count == 0)
            {
                MoveMiningTool();
                SetTargets();
            }
        }

        private void MoveMiningTool()
        {
            if (this.direction == Direction.Left)
            {
                var newPos = worldBehaviour.GetRelativePosition(pos, -1, 0);
                if (newPos == -1)
                {
                    StopMining();        
                }
                var p = transform.position;
                transform.position = new Vector3(p.x -1 , p.y, p.z);
                pos = newPos;
            } else if (this.direction == Direction.Down)
            {
                var newPos = worldBehaviour.GetRelativePosition(pos, 0, 1);
                if (newPos == -1)
                {
                    StopMining();
                }
                var p = transform.position;
                transform.position = new Vector3(p.x, p.y - 1, p.z);
                pos = newPos;
            }
        }

        private bool SetTargets()
        {
            if (this.direction == Direction.Left)
            {
                for (int i = 0; i < miningTool.Size.Y; i++)
                {
                    var target = worldBehaviour.GetTile(pos, -1, i);
                    if (target != null && target.IsDigable())
                    {
                        targets.Add((GroundBehaviour)target);
                    }
                }
            }
            else if (this.direction == Direction.Down)
            {
                for (int i = 0; i < miningTool.Size.X; i++)
                {
                    var target = worldBehaviour.GetTile(pos, i, 1);
                    if (target != null && target.IsDigable())
                    {
                        targets.Add((GroundBehaviour)target);
                    }
                }
            }

            if (targets.Count < 1)
            {
                StopMining();
                return false;
            }
            return true;
        }

        public void StartMining()
        {
            if (SetTargets())
            {
                isMining = true;
            }
        }

        public void StopMining()
        {
            isMining = false;
        }

        public void OnClicked()
        {
            Debug.Log("Open Menu");
            isMining = !isMining;
        }

    }
}
