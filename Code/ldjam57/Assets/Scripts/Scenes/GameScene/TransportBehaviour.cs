using Assets.Scripts.Core.Model;
using GameFrame.Core.Math;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class TransportBehaviour : MonoBehaviour, IClickable, IStorage
    {
        private Transporter transporter;
        private TransportRoute transportRoute;
        private ShaftBehaviour shaftBehaviour;
        private WorldBehaviour worldBehaviour;
        private float tickInterval = 1f;
        private IStorage from;
        private IStorage to;
        private Direction direction;

        private void Update()
        {
            transporter.Tick -= Time.deltaTime;
            if (transporter.Tick < 0f)
            {
                MoveStuff();
                transporter.Tick = tickInterval;
            }
        }



        public void Init(WorldBehaviour worldBehaviour, ShaftBehaviour shaftBehaviour, Transport transport, Direction direction)
        {
            this.shaftBehaviour = shaftBehaviour;
            this.worldBehaviour = worldBehaviour;
            this.direction = direction;

            Renderer renderer = gameObject.GetComponent<Renderer>();
            Texture2D texture2D = GameFrame.Base.Resources.Manager.Textures.Get(transport.Sprite);
            renderer.material.mainTexture = texture2D;
            transporter = new Transporter()
            {
                Transport = transport,
                Direction = direction,
                Position = shaftBehaviour.GetPosition()
            };
            Base.Core.Game.State.ActiveTransporters.Add(transporter);
            SetStorages();
        }

        private void SetStorages()
        {
            if (direction == Direction.Left)
            {
                from = GetStorage(-1, 0);
                to = GetStorage(1, 0);
            }
            else if (direction == Direction.Right)
            {
                from = GetStorage(1, 0);
                to = GetStorage(-1, 0);
            }
            else if (direction == Direction.Down)
            {
                from = GetStorage(0, 1);
                to = GetStorage(0, -1);
            }
        }

        private void UpdateDigger(DiggerBehaviour digger)
        {
            SetStorages();
            digger.OnDiggerMoved.RemoveListener(UpdateDigger);
        }

        private IStorage GetStorage(int x, int y)
        {
            var shaft = worldBehaviour.GetTilerRelativeOfType<ShaftBehaviour>(transporter.Position, x, y);
            if (shaft == null)
            {
                return null;
            }
            if (shaft.DiggerBehaviour != null)
            {
                shaft.DiggerBehaviour.OnDiggerMoved.AddListener(UpdateDigger);
                return shaft.DiggerBehaviour;
            }
            if (shaft.TransportBehaviour != null)
            {
                return shaft.TransportBehaviour;
            }
            return null;
        }

        private void MoveStuff()
        {
            double amountToMove = transporter.Transport.Speed;
            if (from != null && to != null)
            {
                var movedAmount = StorageHelper.MoveStuff(from, to, amountToMove);
                amountToMove -= movedAmount;
                if (amountToMove <= 0)
                {
                    return;
                }
            }
            if (to != null)
            {
                var movedAmount = StorageHelper.MoveStuff(this, to, amountToMove);
                amountToMove -= movedAmount;
                if (amountToMove <= 0)
                {
                    return;
                }
            }
            if (from != null)
            {
                var movedAmount = StorageHelper.MoveStuff(from, this, amountToMove);
                amountToMove -= movedAmount;
                if (amountToMove <= 0)
                {
                    return;
                }
            }
        }



        public void OnClicked()
        {

        }

        public Point2 GetPosition()
        {
            return shaftBehaviour.GetPosition();
        }

        public Dictionary<Mineral, double> GetStorage()
        {
            return transporter.StoredAmount;
        }

        public double AmountStored()
        {
            throw new NotImplementedException();
        }
    }
}
