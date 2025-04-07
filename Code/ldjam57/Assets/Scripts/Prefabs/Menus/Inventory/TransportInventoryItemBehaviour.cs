using System;
using System.Collections.Generic;

using Assets.Scripts.Core.Model.Inventories;

namespace Assets.Scripts.Prefabs.Menus.Inventory
{
    public class TransportInventoryItemBehaviour : InventoryItemBehaviour<TransportInventoryItem>
    {
        public void Init(List<TransportInventoryItem> items, Boolean isVertical)
        {
            base.Init(items);
        }
    }
}
