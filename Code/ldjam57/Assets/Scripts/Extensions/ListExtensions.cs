using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Core.Model;

using GameFrame.Core.Extensions;

namespace Assets.Scripts.Extensions
{
    public static class ListExtensions
    {
        public static void AddOrUpdate<TItem>(this List<TItem> list, TItem item) where TItem : InventoryItem
        {
            if (list != default)
            {
                var key = item?.GetKey();

                if (key.HasValue())
                {
                    var currentItem = list.FirstOrDefault(t => t.GetKey() == key);

                    if (currentItem == default)
                    {
                        list.Add(item);
                    }
                    else
                    {
                        currentItem.Amount += item.Amount;
                    }
                }
            }
        }
    }
}
