using Assets.Scripts.Core.Model;
using GameFrame.Core.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace Assets.Scripts.Scenes.GameScene
{
    public class TransportBehaviour : MonoBehaviour, IClickable, IStorage
    {
        [SerializeField]
        private TextMeshPro testLabel;

        private Transporter transporter;
        private TransportRoute transportRoute;
        private ShaftBehaviour shaftBehaviour;
        private WorldBehaviour worldBehaviour;
        private float tickInterval = 1f;
        private Direction direction;



        private void Update()
        {
            transporter.Tick -= Time.deltaTime;
            if (transporter.Tick < 0f)
            {
                MoveStuff();
                transporter.Tick = tickInterval;
                UpdateTest();
            }
        }

        private void UpdateTest()
        {
            List<string> values = new();
            foreach (var pair in transporter.StoredAmount)
            {
                double weight = pair.Value * pair.Key.Weight;
                values.Add(pair.Key.Name + ": " + weight.ToString("F6"));
            }
            var testText = GameFrame.Core.Json.Handler.SerializePretty(values);
            testLabel.text = testText;
        }

        public void Init(WorldBehaviour worldBehaviour, ShaftBehaviour shaftBehaviour, Transport transport, Direction direction)
        {
            this.shaftBehaviour = shaftBehaviour;
            this.worldBehaviour = worldBehaviour;
            this.direction = direction;

            SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
            Sprite texture2D = GameFrame.Base.Resources.Manager.Sprites.Get(transport.Sprite);
            renderer.sprite = texture2D;
            if (direction == Direction.Right)
            {
                renderer.flipX = true;
            }
            transporter = new Transporter()
            {
                Transport = transport,
                Direction = direction,
                Position = shaftBehaviour.GetPosition()
            };
            Base.Core.Game.State.ActiveTransporters.Add(transporter);
            //SetStorages();
            RegisterStorage();
            playSoundEffect();
        }


        private void UpdateDigger(DiggerBehaviour digger)
        {
            digger.OnDiggerMoved.RemoveListener(UpdateDigger);
        }

        private void MoveStuff()
        {
            var p = transporter.Position;
            var storages = GetStorages(p);

            if (storages.Count == 1)
            {
                return;
            }
            //if (transporter.Direction == Direction.Left)
            //{
            //    Debug.Log("Horizontal");
            //}
            double amountToMove = transporter.Transport.Speed;
            int cnt = 10;
            while (cnt > 0)
            {
                bool moved = false;
                int halfPoint = storages.Count / 2;
                for (int f = 0; f < halfPoint; f++)
                {
                    for (int t = storages.Count - 1; t >= halfPoint - 1; t--)
                    {
                        var from = storages[f];
                        var to = storages[t];
                        if (to == from || !from.CanBeTakenFrom() || !to.CanBePutInto())
                        {
                            continue;
                        }
                        var movedAmount = StorageHelper.MoveStuff(from, to, amountToMove);
                        if (movedAmount > 0)
                        {
                            moved = true;
                        }
                        amountToMove -= movedAmount;
                        if (amountToMove <= 0.001)
                        {
                            return;
                        }
                    }
                }
                if (!moved)
                {
                    return;
                }
                cnt--;
                if (cnt < 5) {
                    Debug.Log("Strange stuff hapening");
                }
            }
            Debug.LogError("Error in Transporter, to many move attemts");
        }

        private void AddStorages(int x, int y, List<IStorage> storages)
        {
            if (worldBehaviour.GetStoragesAtPosition(x, y, out var tmps))
            {
                storages.AddRange(tmps);
            }
        }


        private List<IStorage> GetStorages(Point2 p)
        {
            if (direction == Direction.Left || direction == Direction.Right)
            {
                List<IStorage> storages = new List<IStorage>();
                AddStorages(p.X - 1, p.Y, storages);
                AddStorages(p.X, p.Y, storages);
                AddStorages(p.X + 1, p.Y, storages);
                if (direction == Direction.Left)
                {
                    return storages.OrderBy(s => s.Priority()).ThenBy(s => s.GetPosition().X).ToList();
                }
                else {
                    return storages.OrderBy(s => s.Priority()).ThenByDescending(s => s.GetPosition().X).ToList();
                }
            }
            else if (direction == Direction.Down)
            {
                List<IStorage> storages = new List<IStorage>();
                AddStorages(p.X, p.Y - 1, storages);
                AddStorages(p.X, p.Y, storages);
                AddStorages(p.X, p.Y + 1, storages);
                
                return storages.OrderBy(s => s.Priority()).ThenByDescending(s => s.GetPosition().Y).ToList();                
            }
            return new();
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

        void RegisterStorage()
        {
            worldBehaviour.RegisterStorage(this);
        }

        void UnRegisterStorage()
        {
            worldBehaviour.UnRegisterStorage(this);
        }


        void IStorage.RegisterStorage()
        {
            RegisterStorage();
        }

        void IStorage.UnRegisterStorage()
        {
            UnRegisterStorage();
        }

        public double GetCapacity()
        {
            return transporter.Transport.Capacity;
        }

        public int Priority()
        {
            return -1;
        }

        private void playSoundEffect()
        {
            if (!string.IsNullOrEmpty(transporter.Transport.Sound))
            {
                var audioClip = GameFrame.Base.Resources.Manager.Audio.Get(transporter.Transport.Sound);
                if (audioClip != null)
                {
                    GameFrame.Base.Audio.Effects.Play(audioClip);
                }
            }
        }
    }
}
