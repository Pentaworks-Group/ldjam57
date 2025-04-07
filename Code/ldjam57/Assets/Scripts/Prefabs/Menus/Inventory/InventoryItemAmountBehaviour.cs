using System;

using Assets.Scripts.Core.Model;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Prefabs.Menus.Inventory
{
    public class InventoryItemAmountBehaviour : MonoBehaviour
    {
        private InventoryItem inventorItem;
        private Int32 runningNumber;
        private Boolean? buttonWasInteractable;

        [SerializeField]
        private TMP_Text text;
        [SerializeField]
        private Button button;

        public void Init(InventoryItem inventoryItem)
        {
            this.inventorItem = inventoryItem;
        }

        public void Update()
        {
            if (inventorItem != default && Base.Core.Game.IsRunning)
            {
                if (runningNumber != inventorItem.Amount)
                {
                    runningNumber = inventorItem.Amount;
                    this.text.text = runningNumber.ToString();

                    button.interactable = runningNumber > 0;
                }
            }
        }

        internal void ForceDisable()
        {
            if (!this.buttonWasInteractable.HasValue)
            {
                this.buttonWasInteractable = button.interactable;
                button.interactable = false;
            }
            else
            {
                Debug.LogError("-- Button was forcefully disabled but not restored!!! ---");
            }
        }

        internal void RestoreStatus()
        {
            if (this.buttonWasInteractable.HasValue)
            {
                button.interactable = buttonWasInteractable.Value;

                this.buttonWasInteractable = null;
            }
            else
            {
                Debug.LogError("-- Button was not forcefully disabled before attempting a restore!!! ---");
            }
        }
    }
}
