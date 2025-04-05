using Assets.Scripts.Core.Model;
using GameFrame.Core.Math;
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


        public Direction GetDirection()
        {
            return direction;
        }


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

        public void Init(WorldBehaviour worldBehaviour, MiningTool minigTool, Direction direction, Point2 pos)
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
                var validPos = worldBehaviour.GetRelativePosition(pos, -1, 0, out Point2 newPoint);
                if (!validPos)
                {
                    StopMining();        
                }
                var p = transform.position;
                transform.position = new UnityEngine.Vector3(p.x -1 , p.y, p.z);
                pos = newPoint;
            } else if (this.direction == Direction.Right)
            {
                var validPos = worldBehaviour.GetRelativePosition(pos, 1, 0, out Point2 newPoint);
                if (!validPos)
                {
                    StopMining();
                }
                var p = transform.position;
                transform.position = new UnityEngine.Vector3(p.x + 1, p.y, p.z);
                pos = newPoint;
            }
            else if (this.direction == Direction.Down)
            {
                var validPos = worldBehaviour.GetRelativePosition(pos, 0, 1, out Point2 newPoint);
                if (!validPos)
                {
                    StopMining();
                }
                var p = transform.position;
                transform.position = new UnityEngine.Vector3(p.x, p.y - 1, p.z);
                pos = newPoint;
            }
            worldBehaviour.DiggerMoved(this);
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
            else if (this.direction == Direction.Right)
            {
                for (int i = 0; i < miningTool.Size.Y; i++)
                {
                    var target = worldBehaviour.GetTile(pos, 1, i);
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
            Destroy(gameObject);
        }

    }
}
