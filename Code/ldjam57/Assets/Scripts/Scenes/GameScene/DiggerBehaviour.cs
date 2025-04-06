using System;
using System.Collections.Generic;

using Assets.Scripts.Core.Model;

using GameFrame.Core.Math;

using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class DiggerBehaviour : TileBehaviour, IClickable, IStorage
    {
        private Digger digger;
        private List<GroundBehaviour> targets = new List<GroundBehaviour>();

        private float tickInterval = 1f;
        private float xOffset;
        private float yOffset;
        private double usedCapacity = 0;

        private ShaftBehaviour shaft;

        private Animator _animator;

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
            _animator = gameObject.GetComponent<Animator>();
            _animator.gameObject.SetActive(true);
            StartMining();
        }

        public void UpdatePosition()
        {
            var posi = worldBehaviour.GetUnityVector(digger.Position, -0.01f, xOffset, yOffset);
            transform.position = posi;
            if (shaft != null)
            {
                shaft.SetDigger(null);
            }
            ShaftBehaviour newShaft = (ShaftBehaviour)worldBehaviour.GetTileRelative(digger.Position, 0, 0);
            if (newShaft != null)
            {
                newShaft.SetDigger(this);
            }
        }


        private void MineTargets()
        {
            if (usedCapacity < digger.MiningTool.Capacity)
            {
                for (int i = targets.Count - 1; i >= 0; i--)
                {
                    if (targets[i].UpdateProgress(digger.MiningTool, out List<MineralAmount> mined))
                    {
                        targets.RemoveAt(i);
                    }
                    StoreMinerals(mined);
                }
                if (targets.Count == 0)
                {
                    MoveDigger();
                    SetTargets();
                }
            }
        }

        private void StoreMinerals(List<MineralAmount> mined)
        {
            foreach (MineralAmount mineralA in mined)
            {
                if (digger.MinedAmount.ContainsKey(mineralA.Mineral))
                {
                    digger.MinedAmount[mineralA.Mineral] += mineralA.Amount;
                }
                else
                {
                    digger.MinedAmount[mineralA.Mineral] = mineralA.Amount;
                }
            }
            usedCapacity = 0;
            foreach (var pair in digger.MinedAmount)
            {
                usedCapacity += pair.Value * pair.Key.Weight;
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
                _animator.SetBool("Mining", true);
                int direction = 0;
                if (digger.Direction == Direction.Left)
                    direction = -1;
                else if (digger.Direction == Direction.Right)
                    direction = 1;
                _animator.SetInteger("Direction", direction);
            }
        }

        public void StopMining()
        {
            digger.IsMining = false;
            _animator.SetBool("Mining", false);
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

        public Dictionary<Mineral, double> GetStorage()
        {
            return digger.MinedAmount;
        }
    }
}
