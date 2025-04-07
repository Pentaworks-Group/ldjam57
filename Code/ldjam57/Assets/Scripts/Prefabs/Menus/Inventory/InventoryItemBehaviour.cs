using System;
using System.Collections.Generic;

using Assets.Scripts.Core.Model;

using GameFrame.Core.Extensions;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Prefabs.Menus.Inventory
{
    public abstract class InventoryItemBehaviour<TInventoryItem> : MonoBehaviour, IInventoryItemBehaviour where TInventoryItem : InventoryItem
    {
        public Action<IInventoryItemBehaviour> OnIsExpandedChanged;
        public Action<TInventoryItem> OnItemSelected;

        [SerializeField]
        private GameObject expandArea;
        [SerializeField]
        private TMP_Text text;
        [SerializeField]
        private Button toggleButton;

        [SerializeField]
        private GameObject buttonTemplate;
        private List<InventoryItemAmountBehaviour> spawnedButtons = new List<InventoryItemAmountBehaviour>();

        private Boolean isExpanded;
        public Boolean IsExpanded => isExpanded;

        protected List<TInventoryItem> inventoryItems;

        public void EnableButtons()
        {
            this.toggleButton.interactable = true;

            foreach (var spawnedButtons in this.spawnedButtons)
            {
                spawnedButtons.RestoreStatus();
            }
        }

        public void DisableButtons()
        {
            this.toggleButton.interactable = false;

            foreach (var spawnedButtons in this.spawnedButtons)
            {
                spawnedButtons.ForceDisable();
            }
        }

        public virtual void Init(List<TInventoryItem> inventoryItems)
        {
            if (inventoryItems?.Count > 0)
            {
                this.inventoryItems = inventoryItems;

                for (int i = 0; i < inventoryItems.Count; i++)
                {
                    var item = inventoryItems[i];

                    SpawnButton(i, item);
                }

                toggleButton.interactable = true;
            }
            else
            {
                toggleButton.interactable = false;
            }
        }

        public void Close()
        {
            Collapse();
            this.isExpanded = false;
        }

        public void Toggle()
        {
            if (this.isExpanded)
            {
                Collapse();
            }
            else
            {
                Expand();
            }

            this.isExpanded = !this.isExpanded;

            OnIsExpandedChanged?.Invoke(this);
        }

        protected virtual void Expand()
        {
            this.expandArea.SetActive(true);
        }

        protected virtual void Collapse()
        {
            this.expandArea.SetActive(false);
        }

        protected void SpawnButton(Int32 index, TInventoryItem item)
        {
            var newButton = Instantiate(buttonTemplate, expandArea.transform);

            if (newButton.TryGetComponent<InventoryItemAmountBehaviour>(out var amountBehaviour))
            {
                amountBehaviour.Init(item);
            }

            if (newButton.TryGetComponent<RectTransform>(out var rectTransform))
            {
                rectTransform.anchorMin = new Vector2(index, 0);
                rectTransform.anchorMax = new Vector2(index + 1, 1);
            }

            if (newButton.TryGetComponent<Button>(out var button))
            {
                button.onClick.AddListener(() => OnItemSelected?.Invoke(item));
            }

            var spriteName = item.GetSprite();

            if (spriteName.HasValue())
            {
                if (newButton.transform.Find("IconObject").TryGetComponent<Image>(out var image))
                {
                    var loadedSprite = GameFrame.Base.Resources.Manager.Sprites.Get(spriteName);

                    if (loadedSprite != null)
                    {
                        image.sprite = loadedSprite;
                    }
                }
            }

            newButton.name = "Item-" + item.GetName();
            newButton.SetActive(true);
        }

        protected virtual void UpdateInternal()
        {
            UpdateInventoryText();
        }

        private void UpdateInventoryText()
        {
            var total = 0;

            foreach (var item in this.inventoryItems)
            {
                total += item.Amount;
            }

            this.text.text = total.ToString();
        }

        private void Update()
        {
            if (Base.Core.Game.IsRunning)
            {
                UpdateInternal();
            }
        }
    }
}
