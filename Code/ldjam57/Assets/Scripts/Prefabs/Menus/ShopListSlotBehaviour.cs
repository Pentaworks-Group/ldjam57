using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Base;
using Assets.Scripts.Core.Model.Inventories;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

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
                Core.Model.Inventories.MiningToolInventoryItem item = new Core.Model.Inventories.MiningToolInventoryItem();
                item.MiningTool = content.MiningTool;
                Base.Core.Game.State.Inventory.MiningTools.Add(item);
            } else if (content.Type == ShopItemType.Transport && content.Transport != null) {
                TransportInventoryItem item = new TransportInventoryItem();
                item.Transport = content.Transport;

                if (content.TransportDirection == TransportDirection.Horizontal)
                {
                    Base.Core.Game.State.Inventory.HorizontalTransports.Add(item);
                }
                else if (content.TransportDirection == TransportDirection.Vertical)
                {
                    Base.Core.Game.State.Inventory.VerticalTransports.Add(item);
                }
            }

            Base.Core.Game.State.Bank.Credits -= (double)content.PurchaseCost;
        }

    }
}
