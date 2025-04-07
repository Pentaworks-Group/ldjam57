using Assets.Scripts.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Prefabs.Menus
{
    public enum ShopItemType
    {
        Tool,
        Transport
    }

    public enum TransportDirection
    {
        Horizontal,
        Vertical
    }

    public class ShopItem
    {
        public String Name {  get; set; }
        public Decimal PurchaseCost { get; set; }
        public Decimal OperatingCost { get; set; }
        public String Sprite { get; set; }
        public ShopItemType Type { get; set; }
        public MiningTool MiningTool { get; set; } = null;
        public Transport Transport { get; set; } = null;
        public TransportDirection TransportDirection { get; set; }
    }
}
