using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Base;

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
            List<ShopItem> list = new List<ShopItem>();
            Base.Core.Game.State.AvailableMiningTools.ForEach(tool =>
            {
                ShopItem item = new ShopItem();
                item.Name = tool.Name;
                item.PurchaseCost = tool.PurchaseCost;
                item.OperatingCost = tool.OperatingCost;
                item.Sprite = tool.Sprite;
                item.Type = ShopItemType.Tool;
                list.Add(item);
            });

            Base.Core.Game.State.AvailableHorizontalTransports.ForEach(transport =>
            {
                ShopItem item = new ShopItem();
                item.Name = transport.Name;
                item.PurchaseCost = transport.PurchaseCost;
                item.OperatingCost = transport.OperatingCost;
                item.Sprite = transport.Sprite;
                item.Type = ShopItemType.Transport;
                item.TransportDirection = TransportDirection.Horizontal;
            });

            Base.Core.Game.State.AvailableVerticalTransports.ForEach(transport =>
            {
                ShopItem item = new ShopItem();
                item.Name = transport.Name;
                item.PurchaseCost = transport.PurchaseCost;
                item.OperatingCost = transport.OperatingCost;
                item.Sprite = transport.Sprite;
                item.Type = ShopItemType.Transport;
                item.TransportDirection = TransportDirection.Vertical;
            });

            SetContentList(list);
        }
    }
}
