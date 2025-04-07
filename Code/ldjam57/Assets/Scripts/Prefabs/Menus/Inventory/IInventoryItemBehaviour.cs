using System;

namespace Assets.Scripts.Prefabs.Menus.Inventory
{
    public interface IInventoryItemBehaviour
    {
        Boolean IsExpanded { get; }
        void Toggle();
    }
}
