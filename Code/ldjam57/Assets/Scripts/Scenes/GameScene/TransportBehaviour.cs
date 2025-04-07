using Assets.Scripts.Core.Model;
using GameFrame.Core.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
            List<IStorage> froms = new();
            List<IStorage> tos = new();
            GetStorages(p, ref froms, ref tos);

            if (froms.Count == 1 && tos.Count == 1 && froms[0] == tos[0])
            {
                return;
            }
            if (transporter.Direction == Direction.Left)
            {
                Debug.Log("Horizontal");
            }
            double amountToMove = transporter.Transport.Speed;
            int cnt = 10;
            while (cnt > 0)
            {
                bool moved = false;
                foreach (var from in froms)
                {
                    foreach (var to in tos)
                    {
                        if (to == from)
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

        private void GetStorages(Point2 p, ref List<IStorage> froms, ref List<IStorage> tos)
        {
            if (direction == Direction.Left)
            {
                if (!worldBehaviour.GetStoragesAtPosition(p.X - 1, p.Y, out froms) || froms.Count < 1)
                {
                    worldBehaviour.GetStoragesAtPosition(p.X, p.Y, out froms);
                }
                if (!worldBehaviour.GetStoragesAtPosition(p.X + 1, p.Y, out tos) || tos.Count < 1)
                {
                    worldBehaviour.GetStoragesAtPosition(p.X, p.Y, out tos);
                }
            }
            else if (direction == Direction.Right)
            {
                if (!worldBehaviour.GetStoragesAtPosition(p.X + 1, p.Y, out froms) || froms.Count < 1)
                {
                    worldBehaviour.GetStoragesAtPosition(p.X, p.Y, out froms);
                }
                if (!worldBehaviour.GetStoragesAtPosition(p.X - 1, p.Y, out tos) || tos.Count < 1)
                {
                    worldBehaviour.GetStoragesAtPosition(p.X, p.Y, out tos);
                }
            }
            else if (direction == Direction.Down)
            {
                if (!worldBehaviour.GetStoragesAtPosition(p.X, p.Y + 1, out froms) || froms.Count < 1)
                {
                    worldBehaviour.GetStoragesAtPosition(p.X, p.Y, out froms);
                }
                if (!worldBehaviour.GetStoragesAtPosition(p.X, p.Y - 1, out tos) || tos.Count < 1)
                {
                    worldBehaviour.GetStoragesAtPosition(p.X, p.Y, out tos);
                }
            }
            froms = froms.ToList();
            froms.RemoveAll(storag => !storag.CanBeTakenFrom());
            tos = tos.ToList();
            tos.RemoveAll(storag => !storag.CanBePutInto());
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
