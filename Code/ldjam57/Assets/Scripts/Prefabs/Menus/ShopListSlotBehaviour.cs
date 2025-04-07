using Assets.Scripts.Core.Model.Inventories;
using Assets.Scripts.Extensions;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Prefabs.Menus
{
    public class ShopListSlotBehaviour : GameFrame.Core.UI.List.ListSlotBehaviour<ShopItem>
    {
        [SerializeField]
        TextMeshProUGUI nameField;
        [SerializeField]
        TextMeshProUGUI purchaseCostField;
        [SerializeField]
        TextMeshProUGUI operatingCostField;
        [SerializeField]
        Image spriteImage;
        [SerializeField]
        Button buyButton;

        public override void RudeAwake()
        {
            UnityEngine.Debug.Log(content);
        }

        public override void UpdateUI()
        {
            nameField.SetText(content.Name);
            purchaseCostField.SetText(content.PurchaseCost.ToString("F0"));
            operatingCostField.SetText(content.OperatingCost.ToString("F0"));

            if (content.Sprite != null)
            {
                var sprite = GameFrame.Base.Resources.Manager.Sprites.Get(content.Sprite);

                if (sprite != null)
                {
                    spriteImage.sprite = sprite;
                }
                UnityEngine.Debug.Log(sprite);
            }

            if (Base.Core.Game.State.Bank.Credits < (double)content.PurchaseCost)
            {
                buyButton.enabled = false;
            }
            else
            {
                buyButton.enabled = true;
            }
        }

        public void BuyItem()
        {
            //TODO: put in Storage?
            if (content.Type == ShopItemType.Tool && content.MiningTool != null)
            {
                var item = new Core.Model.Inventories.MiningToolInventoryItem
                {
                    MiningTool = content.MiningTool,
                    Amount = 1
                };

                Base.Core.Game.State.Inventory.MiningTools.AddOrUpdate(item);

                GameFrame.Base.Audio.Effects.Play("Buy");
            }
            else if (content.Type == ShopItemType.Transport && content.Transport != null)
            {
                TransportInventoryItem item = new TransportInventoryItem()
                {
                    Transport = content.Transport,
                    Amount = 1,
                };

                if (content.TransportDirection == TransportDirection.Horizontal)
                {
                    item.IsVertical = false;
                    Base.Core.Game.State.Inventory.HorizontalTransports.AddOrUpdate(item);
                }
                else if (content.TransportDirection == TransportDirection.Vertical)
                {
                    item.IsVertical = true;

                    Base.Core.Game.State.Inventory.VerticalTransports.AddOrUpdate(item);
                }

                GameFrame.Base.Audio.Effects.Play("Buy");
            }

            Base.Core.Game.State.Bank.Credits -= (double)content.PurchaseCost;
        }
    }
}
