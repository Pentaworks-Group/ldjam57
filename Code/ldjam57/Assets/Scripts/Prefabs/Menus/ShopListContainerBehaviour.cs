using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Prefabs.Menus
{
    public class ShopListContainerBehaviour : GameFrame.Core.UI.List.ListContainerBehaviour<ShopItem>
    {
        public override void CustomStart()
        {
            UpdateList();
        }

        public void UpdateList()
        {
            UnityEngine.Debug.Log("Test");
        }
    }
}
