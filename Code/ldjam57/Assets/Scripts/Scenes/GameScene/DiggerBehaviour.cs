using System;
using System.Collections.Generic;

using Assets.Scripts.Core.Model;
using Assets.Scripts.Core.Model.Inventories;

using GameFrame.Core.Math;

using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Scenes.GameScene
{
    public class DiggerBehaviour : TileBehaviour, IClickable, IStorage
    {
        [SerializeField]
        private GameObject popUpObject;
        [SerializeField]
        private TextMeshProUGUI nameText;
        [SerializeField]
        private Button upgradeButton;
        [SerializeField]
        private AnimatorOverrideController axeOverrideController;
        [SerializeField]
        private AnimatorOverrideController drillOverrideController;

        private Digger digger;
        private List<GroundBehaviour> targets = new List<GroundBehaviour>();

        private float tickInterval = 1f;
        private float xOffset;
        private float yOffset;

        private ShaftBehaviour shaft;

        private Animator _animator;

        private MiningToolInventoryItem upgradeOption;
        private MiningToolInventoryItem inventoryItem;

        public UnityEvent<DiggerBehaviour> OnDiggerMoved { get; set; } = new UnityEvent<DiggerBehaviour>();

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

            if (digger.Direction == Direction.Left)
            {
                var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                spriteRenderer.flipX = true;
            }

            UpdateAnimator();
            StartMining();
        }

        private void UpdateAnimator()
        {
            if (digger.MiningTool.Reference == "Drill")
            {
                _animator.runtimeAnimatorController = drillOverrideController;
            }
            else
            {
                _animator.runtimeAnimatorController = axeOverrideController;
            }
        }

        public void UpdatePosition()
        {
            var posi = worldBehaviour.GetUnityVector(digger.Position, -0.01f, xOffset, yOffset);
            transform.position = posi;

            if (shaft != null)
            {
                shaft.DiggerBehaviour = null;
            }

            if (worldBehaviour.GetTileRelative<ShaftBehaviour>(digger.Position, 0, 0, out var newShaft))
            {
                newShaft.DiggerBehaviour = this;
                shaft = newShaft;

                RegisterStorage();
            }
        }

        private void MineTargets()
        {
            var storedAmount = StorageHelper.GetStoredAmount(this);
            var storageCapacity = storedAmount / digger.MiningTool.Capacity;

            if (storageCapacity < 1)
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

            //UpdateProgressBar((Single)storageCapacity);
        }

        private void StoreMinerals(List<MineralAmount> mined)
        {
            foreach (MineralAmount mineralA in mined)
            {
                if (digger.MinedAmount.ContainsKey(mineralA.Mineral.Reference))
                {
                    digger.MinedAmount[mineralA.Mineral.Reference].Amount += mineralA.Amount;
                }
                else
                {
                    digger.MinedAmount[mineralA.Mineral.Reference] = new MineralAmount(mineralA.Mineral, mineralA.Amount);
                }
            }
        }

        private void MoveDigger()
        {
            Point2 newPoint;
            switch (digger.Direction)
            {
                case Direction.Left:
                    {
                        var validPos = worldBehaviour.GetRelativePosition(digger.Position, -1, 0, out newPoint);
                        if (!validPos)
                        {
                            StopMining();
                            return;
                        }

                        break;
                    }

                case Direction.Right:
                    {
                        var validPos = worldBehaviour.GetRelativePosition(digger.Position, 1, 0, out newPoint);
                        if (!validPos)
                        {
                            StopMining();
                            return;
                        }

                        break;
                    }

                case Direction.Down:
                    {
                        var validPos = worldBehaviour.GetRelativePosition(digger.Position, 0, 1, out newPoint);
                        if (!validPos)
                        {
                            StopMining();
                            return;
                        }

                        break;
                    }

                default:
                    return;
            }

            UnRegisterStorage();

            digger.Position = newPoint;
            UpdatePosition();

            worldBehaviour.DiggerMoved(this);
            OnDiggerMoved.Invoke(this);
        }

        private bool SetTargets()
        {
            switch (digger.Direction)
            {
                case Direction.Left:
                    {
                        for (var i = 0; i < digger.MiningTool.Size.Y; i++)
                        {
                            if (worldBehaviour.GetTileRelative(digger.Position, -1, i, out var target))
                            {
                                if (target.IsDigable())
                                {
                                    targets.Add((GroundBehaviour)target);
                                }
                            }
                        }

                        break;
                    }

                case Direction.Right:
                    {
                        for (var i = 0; i < digger.MiningTool.Size.Y; i++)
                        {
                            if (worldBehaviour.GetTileRelative(digger.Position, 1, i, out var target))
                            {
                                if (target.IsDigable())
                                {
                                    targets.Add((GroundBehaviour)target);
                                }
                            }
                        }

                        break;
                    }

                case Direction.Down:
                    {
                        for (var i = 0; i < digger.MiningTool.Size.X; i++)
                        {
                            if (worldBehaviour.GetTileRelative(digger.Position, i, 1, out GroundBehaviour target))
                            {
                                if (target.IsDigable())
                                {
                                    targets.Add(target);
                                }
                            }
                        }

                        break;
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
                {
                    direction = -1;
                }
                else if (digger.Direction == Direction.Right)
                {
                    direction = 1;
                }
                _animator.SetInteger("Direction", direction);

                playSoundEffect();
            }
        }

        private MiningToolInventoryItem GetUpgradeOption()
        {
            var mTools = Base.Core.Game.State.Inventory.MiningTools;
            for (int i = 0; i < mTools.Count; i++)
            {
                var tTool = mTools[i];
                if (tTool.MiningTool.Reference == digger.MiningTool.Reference)
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

        public void StopMining()
        {
            digger.IsMining = false;
            _animator.SetBool("Mining", false);
        }

        public void OnClicked()
        {
            OpenPopup();
        }

        public void Upgrade()
        {
            upgradeOption.Amount -= 1;
            inventoryItem.Amount += 1;
            digger.MiningTool = upgradeOption.MiningTool;
            ClosePopup();
            UpdateAnimator();
            playSoundEffect();

        }
        private void OpenPopup()
        {
            nameText.text = digger.MiningTool.Name;
            upgradeOption = GetUpgradeOption();
            upgradeButton.interactable = upgradeOption != null;
            popUpObject.SetActive(true);
        }

        public void ClosePopup()
        {
            popUpObject.SetActive(false);
        }

        public void RemoveDigger()
        {
            Base.Core.Game.State.ActiveDiggers.Remove(digger);
            Base.Core.Game.State.Inventory.MiningTools.Find(m => m.MiningTool.Reference == digger.MiningTool.Reference).Amount += 1;
            Destroy(gameObject);
        }

        public override Point2 GetPosition()
        {
            return digger.Position;
        }

        public Dictionary<String, MineralAmount> GetStorage()
        {
            return digger.MinedAmount;
        }

        public bool CanBePutInto()
        {
            return false;
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
            return digger.MiningTool.Capacity;
        }

        private void playSoundEffect()
        {
            if (!string.IsNullOrEmpty(digger.MiningTool.Sound))
            {
                var audioClip = GameFrame.Base.Resources.Manager.Audio.Get(digger.MiningTool.Sound);
                if (audioClip != null)
                {
                    GameFrame.Base.Audio.Effects.Play(audioClip);
                }
            }
        }

        public int Priority()
        {
            return -1;
        }
    }
}
