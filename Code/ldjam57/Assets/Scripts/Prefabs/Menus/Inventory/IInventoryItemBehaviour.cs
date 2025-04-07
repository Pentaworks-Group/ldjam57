using System;

namespace Assets.Scripts.Prefabs.Menus.Inventory
{
    public interface IInventoryItemBehaviour
    {
        Boolean IsExpanded { get; }

        void EnableButtons();
        void DisableButtons();
        void Toggle();
        void Close();
    }
}
