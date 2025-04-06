using Assets.Scripts.Core.Model;
using GameFrame.Core.Math;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class DiggerBehaviour : TileBehaviour, IClickable
    {
        private Digger digger;
        private List<GroundBehaviour> targets = new List<GroundBehaviour>();

        private float tickInterval = 1f;


        public Direction GetDirection()
        {
            return digger.Direction;
        }

        public GameFrame.Core.Math.Vector2 GetSize()
        {
            return digger.MiningTool.Size;
        }


        private void Awake()
        {
            this.digable = false;
            digger.Tick = tickInterval;
        }

        private void Update()
        {
            if (digger.IsMining)
            {
                digger.Tick -= Time.deltaTime;
                if (digger.Tick < 0f)
                {
                    MineTargets();
                    digger.Tick = tickInterval;
                }
            }
        }

        public void Init(WorldBehaviour worldBehaviour, Digger digger)
        {
            base.Init(worldBehaviour, digger.Position);
            this.digger = digger;
            StartMining();
        }

        private void MineTargets()
        {
            for (int i = targets.Count - 1; i >= 0; i--)
            {
                if (targets[i].UpdateProgress(digger.MiningTool))
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
            if (digger.Direction == Direction.Left)
            {
                var validPos = worldBehaviour.GetRelativePosition(pos, -1, 0, out Point2 newPoint);
                if (!validPos)
                {
                    StopMining();
                }
                var p = transform.position;
                transform.position = new UnityEngine.Vector3(p.x - 1, p.y, p.z);
                pos = newPoint;
            }
            else if (digger.Direction == Direction.Right)
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
            else if (digger.Direction == Direction.Down)
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
            if (digger.Direction == Direction.Left)
            {
                for (int i = 0; i < digger.MiningTool.Size.Y; i++)
                {
                    var target = worldBehaviour.GetTileRelative(pos, -1, i);
                    if (target != null && target.IsDigable())
                    {
                        targets.Add((GroundBehaviour)target);
                    }
                }
            }
            else if (digger.Direction == Direction.Right)
            {
                for (int i = 0; i < digger.MiningTool.Size.Y; i++)
                {
                    var target = worldBehaviour.GetTileRelative(pos, 1, i);
                    if (target != null && target.IsDigable())
                    {
                        targets.Add((GroundBehaviour)target);
                    }
                }
            }
            else if (digger.Direction == Direction.Down)
            {
                for (int i = 0; i < digger.MiningTool.Size.X; i++)
                {
                    var target = worldBehaviour.GetTileRelative(pos, i, 1);
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
                digger.IsMining = true;
            }
        }

        public void StopMining()
        {
            digger.IsMining = false;
        }

        public void OnClicked()
        {
            RemoveDigger();
        }

        private void RemoveDigger()
        {
            Base.Core.Game.State.ActiveDiggers.Remove(digger);
            Destroy(gameObject);
        }
    }
}
