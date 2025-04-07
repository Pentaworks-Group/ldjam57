using System.Collections.Generic;

using Assets.Scripts.Core.Model;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Prefabs.Menus.Inventory
{
    public class InventoryMenuBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private readonly List<IInventoryItemBehaviour> registeredInventoryItemBehaviours = new List<IInventoryItemBehaviour>();
        private Assets.Scripts.Core.Model.Inventory inventory;

        [SerializeField]
        private MiningToolInventoryItemBehaviour miningToolItemsMenu;
        [SerializeField]
        private TransportInventoryItemBehaviour horizontalTransportItemsMenu;
        [SerializeField]
        private TransportInventoryItemBehaviour verticalTransportItemsMenu;

        public UnityEvent<InventoryItem> OnItemSelected = new UnityEvent<InventoryItem>();
        public UnityEvent PointerEntered = new UnityEvent();
        public UnityEvent PointerExited = new UnityEvent();

        private void Awake()
        {
            Base.Core.Game.ExecuteAfterInstantation(LoadInventory);
        }

        private void LoadInventory()
        {
            if (Base.Core.Game.State != default)
            {
                this.inventory = Base.Core.Game.State.Inventory;

                Register(this.miningToolItemsMenu, this.inventory.MiningTools);
                Register(this.horizontalTransportItemsMenu, this.inventory.HorizontalTransports);
                Register(this.verticalTransportItemsMenu, this.inventory.VerticalTransports);
            }
        }

        private void Register<TInventoryItem>(InventoryItemBehaviour<TInventoryItem> behaviour, List<TInventoryItem> items) where TInventoryItem : InventoryItem
        {
            if (behaviour != null)
            {
                behaviour.Init(items);
                behaviour.OnIsExpandedChanged = OnInvenoryItemExpanded;
                behaviour.OnItemSelected = InventoryItem_OnItemSelected;

                registeredInventoryItemBehaviours.Add(behaviour);
            }
        }

        private void OnInvenoryItemExpanded(IInventoryItemBehaviour changedBehaviour)
        {
            if (changedBehaviour.IsExpanded)
            {
                foreach (var registeredBehaviour in this.registeredInventoryItemBehaviours)
                {
                    if (registeredBehaviour != changedBehaviour)
                    {
                        if (registeredBehaviour.IsExpanded)
                        {
                            registeredBehaviour.Toggle();
                        }
                    }
                }
            }
        }

        private void InventoryItem_OnItemSelected(InventoryItem inventoryItem)
        {
            this.OnItemSelected.Invoke(inventoryItem);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.PointerEntered.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.PointerExited.Invoke();
        }
    }
}
