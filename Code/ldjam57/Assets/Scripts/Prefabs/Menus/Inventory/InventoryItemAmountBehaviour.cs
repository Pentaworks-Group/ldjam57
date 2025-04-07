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
    }
}
