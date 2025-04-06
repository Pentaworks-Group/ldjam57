using Assets.Scripts.Core.Model;
using GameFrame.Core.Math;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;

namespace Assets.Scripts.Scenes.GameScene
{
    public class DiggerBehaviour : TileBehaviour, IClickable
    {
        private Digger digger;
        private List<GroundBehaviour> targets = new List<GroundBehaviour>();

        private float tickInterval = 1f;
        private float xOffset;
        private float yOffset;

        

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
            base.Init(worldBehaviour);
            this.digger = digger;
            CalculateOffsetsAndRotation();
            StartMining();
        }

        public void UpdatePosition()
        {
            var posi = worldBehaviour.GetUnityVector(digger.Position, xOffset, yOffset);
            transform.position = posi;
        }


        private void CalculateOffsetsAndRotation()
        {
            if(digger.Direction == Direction.Down)
            {
                xOffset = 0;
                yOffset = 1 - transform.localScale.y;
            }
            else if (digger.Direction == Direction.Left)
            {
                xOffset = transform.localScale.y - 1;
                yOffset = 0;
                transform.rotation *= Quaternion.Euler(0, 0, -90);
            }
            else if (digger.Direction == Direction.Right)
            {
                xOffset = 1 - transform.localScale.y;
                yOffset = 0;
                transform.rotation *= Quaternion.Euler(0, 0, 90);
            }
            else
            {
                return;
            }
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
                MoveDigger();
                SetTargets();
            }
        }

        private void MoveDigger()
        {

            if (digger.Direction == Direction.Left)
            {
                var validPos = worldBehaviour.GetRelativePosition(digger.Position, -1, 0, out Point2 newPoint);
                if (!validPos)
                {
                    StopMining();
                    return;
                }
                digger.Position = newPoint;
            }
            else if (digger.Direction == Direction.Right)
            {
                var validPos = worldBehaviour.GetRelativePosition(digger.Position, 1, 0, out Point2 newPoint);
                if (!validPos)
                {
                    StopMining();
                    return;
                }
                digger.Position = newPoint;
            }
            else if (digger.Direction == Direction.Down)
            {
                var validPos = worldBehaviour.GetRelativePosition(digger.Position, 0, 1, out Point2 newPoint);
                if (!validPos)
                {
                    StopMining();
                    return;
                }
                digger.Position = newPoint;
            }
            UpdatePosition();
            worldBehaviour.DiggerMoved(this);
        }

        private bool SetTargets()
        {
            if (digger.Direction == Direction.Left)
            {
                for (int i = 0; i < digger.MiningTool.Size.Y; i++)
                {
                    var target = worldBehaviour.GetTileRelative(digger.Position, -1, i);
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
                    var target = worldBehaviour.GetTileRelative(digger.Position, 1, i);
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
                    var target = worldBehaviour.GetTileRelative(digger.Position, i, 1);
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

        public override Point2 GetPosition()
        {
           return digger.Position;
        }
    }
}
