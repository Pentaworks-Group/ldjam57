using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Core.Model;
using Assets.Scripts.Core.Model.Inventories;

using GameFrame.Core.Math;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Scenes.GameScene
{
    public class TransportBehaviour : MonoBehaviour, IClickable, IStorage
    {
        [SerializeField]
        private TextMeshPro testLabel;
        [SerializeField]
        private GameObject popUpObject;
        [SerializeField]
        private TextMeshProUGUI nameText;
        [SerializeField]
        private Button upgradeButton;

        private Transporter transporter;
        private TransportRoute transportRoute;
        private ShaftBehaviour shaftBehaviour;
        private WorldBehaviour worldBehaviour;
        private float tickInterval = 1f;

        private TransportInventoryItem upgradeOption;
        private TransportInventoryItem inventoryItem;

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
            List<String> values = new();

            foreach (var pair in transporter.StoredAmount)
            {
                double weight = pair.Value.Amount * pair.Value.Mineral.Weight;
                values.Add(pair.Value.Mineral.Name + ": " + weight.ToString("F6"));
            }

            var testText = GameFrame.Core.Json.Handler.SerializePretty(values);
            testLabel.text = testText;
        }

        public void Init(WorldBehaviour worldBehaviour, ShaftBehaviour shaftBehaviour, Transport transport, Direction direction)
        {
            this.shaftBehaviour = shaftBehaviour;
            this.worldBehaviour = worldBehaviour;


            transporter = new Transporter()
            {
                Transport = transport,
                Direction = direction,
                Position = shaftBehaviour.GetPosition()
            };

            UpdateSprite();
            Base.Core.Game.State.ActiveTransporters.Add(transporter);
            //SetStorages();
            RegisterStorage();
            playSoundEffect();
        }

        private void UpdateSprite()
        {
            if (gameObject.transform.Find("SpritePlacement")?.TryGetComponent<SpriteRenderer>(out var renderer) == true)
            {
                Sprite texture2D = GameFrame.Base.Resources.Manager.Sprites.Get(transporter.Transport.Sprite);

                renderer.sprite = texture2D;

                if (transporter.Direction == Direction.Right)
                {
                    renderer.flipX = true;
                }
            }
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
                transporter.IsActive = false;
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
                            transporter.IsActive = true;
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
                if (cnt < 5)
                {
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
            if (transporter.Direction == Direction.Left || transporter.Direction == Direction.Right)
            {
                List<IStorage> storages = new List<IStorage>();
                AddStorages(p.X - 1, p.Y, storages);
                AddStorages(p.X, p.Y, storages);
                AddStorages(p.X + 1, p.Y, storages);
                if (transporter.Direction == Direction.Left)
                {
                    return storages.OrderBy(s => s.Priority()).ThenBy(s => s.GetPosition().X).ToList();
                }
                else
                {
                    return storages.OrderBy(s => s.Priority()).ThenByDescending(s => s.GetPosition().X).ToList();
                }
            }
            else if (transporter.Direction == Direction.Down)
            {
                List<IStorage> storages = new List<IStorage>();
                AddStorages(p.X, p.Y - 1, storages);
                AddStorages(p.X, p.Y, storages);
                AddStorages(p.X, p.Y + 1, storages);

                return storages.OrderBy(s => s.Priority()).ThenByDescending(s => s.GetPosition().Y).ToList();
            }
            return new();
        }

        private TransportInventoryItem GetUpgradeOption()
        {
            List<TransportInventoryItem> mTools;
            if (transporter.Direction == Direction.Down)
            {
                mTools = Base.Core.Game.State.Inventory.VerticalTransports;
            }
            else
            {
                mTools = Base.Core.Game.State.Inventory.HorizontalTransports;
            }
            for (int i = 0; i < mTools.Count; i++)
            {
                var tTool = mTools[i];
                if (tTool.Transport.Reference == transporter.Transport.Reference)
                {
                    inventoryItem = tTool;
                    var nI = i + 1;
                    if (nI < mTools.Count && mTools[nI].Amount > 0)
                    {
                        return mTools[nI];
                    }
                    return null;
                }
            }
            return null;
        }


        public void OnClicked()
        {
            OpenPopup();
        }


        public void Upgrade()
        {
            upgradeOption.Amount -= 1;
            inventoryItem.Amount += 1;
            transporter.Transport = upgradeOption.Transport;
            UpdateSprite();
            ClosePopup();
            playSoundEffect();

        }
        private void OpenPopup()
        {
            nameText.text = transporter.Transport.Name;
            upgradeOption = GetUpgradeOption();
            upgradeButton.interactable = upgradeOption != null;
            popUpObject.SetActive(true);
        }

        public void RemoveTransporter()
        {
            Base.Core.Game.State.ActiveTransporters.Remove(transporter);
            if (transporter.Direction == Direction.Down)
            {
                Base.Core.Game.State.Inventory.VerticalTransports.Find(m => m.Transport.Reference == transporter.Transport.Reference).Amount += 1;
            }
            else
            {
                Base.Core.Game.State.Inventory.HorizontalTransports.Find(m => m.Transport.Reference == transporter.Transport.Reference).Amount += 1;
            }
            Destroy(gameObject);
        }


        public void ClosePopup()
        {
            popUpObject.SetActive(false);
        }

        public Point2 GetPosition()
        {
            return shaftBehaviour.GetPosition();
        }

        public Dictionary<String, MineralAmount> GetStorage()
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
