using System.Collections.Generic;

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
                ShopItem item = new ShopItem
                {
                    MiningTool = tool,
                    Name = tool.Name,
                    PurchaseCost = tool.PurchaseCost,
                    OperatingCost = tool.OperatingCost,
                    Sprite = tool.Sprite,
                    Type = ShopItemType.Tool
                };

                list.Add(item);
            });

            Base.Core.Game.State.AvailableHorizontalTransports.ForEach(transport =>
            {
                ShopItem item = new ShopItem
                {
                    Transport = transport,
                    Name = transport.Name,
                    PurchaseCost = transport.PurchaseCost,
                    OperatingCost = transport.OperatingCost,
                    Sprite = transport.Sprite,
                    Type = ShopItemType.Transport,
                    TransportDirection = TransportDirection.Horizontal
                };

                list.Add(item);
            });

            Base.Core.Game.State.AvailableVerticalTransports.ForEach(transport =>
            {
                ShopItem item = new ShopItem
                {
                    Transport = transport,
                    Name = transport.Name,
                    PurchaseCost = transport.PurchaseCost,
                    OperatingCost = transport.OperatingCost,
                    Sprite = transport.Sprite,
                    Type = ShopItemType.Transport,
                    TransportDirection = TransportDirection.Vertical
                };

                list.Add(item);
            });

            list.Sort();

            SetContentList(list);
        }
    }
}
