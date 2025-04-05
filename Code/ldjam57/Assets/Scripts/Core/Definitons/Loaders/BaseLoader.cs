using System;
using System.Collections;
using System.Collections.Generic;

using GameFrame.Core.Definitions;
using GameFrame.Core.Definitions.Loaders;

using Unity.VisualScripting;

namespace Assets.Scripts.Core.Definitons.Loaders
{
    public class BaseLoader<TDefinition> : DefinitionLoader<TDefinition> where TDefinition : BaseDefinition
    {
        public BaseLoader(DefinitionCache<TDefinition> targetCache) : base(targetCache)
        { }

        protected virtual TItem CheckItem<TItem>(TItem loadedItem, DefinitionCache<TItem> referenceCache) where TItem : GameFrame.Core.Definitions.BaseDefinition, new()
        {
            var targetItem = new TItem()
            {
                Reference = loadedItem.Reference
            };

            if (loadedItem.IsReferenced)
            {
                if (referenceCache.TryGetValue(loadedItem.Reference, out var referencedItem))
                {
                    foreach (var property in loadedItem.GetType().GetProperties())
                    {
                        if (property.Name != nameof(GameFrame.Core.Definitions.BaseDefinition.IsReferenced))
                        {
                            if (property.PropertyType.IsGenericType && (typeof(IList).IsAssignableFrom(property.PropertyType.GetGenericTypeDefinition())))
                            {
                                var listValue = property.GetValue(loadedItem);

                                var loadedList = listValue;

                                if (listValue == default || listValue is IList sourceList && sourceList.Count == 0)
                                {
                                    listValue = property.GetValue(referencedItem);
                                }

                                var newList = (IList)Activator.CreateInstance(property.PropertyType);

                                if (listValue is IList list)
                                {
                                    foreach (var item in list)
                                    {
                                        _ = newList.Add(item);
                                    }
                                }

                                var referencedList = property.GetValue(referencedItem);

                                property.SetValue(targetItem, newList);
                            }
                            else if (property.PropertyType.IsNullable())
                            {
                                var actualValue = property.GetValue(loadedItem);

                                var loadedValue = actualValue;

                                if (actualValue == default)
                                {
                                    actualValue = property.GetValue(referencedItem);
                                }

                                var referenceValue = property.GetValue(referencedItem);

                                property.SetValue(targetItem, actualValue);
                            }
                            else
                            {
                                throw new NotSupportedException(String.Format("Unsupported Property '{0}' on '{1}", property.Name, typeof(TItem).FullName));
                            }
                        }
                    }
                }
                else
                {
                    throw new KeyNotFoundException(String.Format("Could't find Object with reference '{0}' ('{1}') in cache!", loadedItem.Reference, typeof(TItem).FullName));
                }
            }
            else
            {
                foreach (var property in loadedItem.GetType().GetProperties())
                {
                    if (property.PropertyType.IsGenericType && (typeof(IList).IsAssignableFrom(property.PropertyType.GetGenericTypeDefinition())))
                    {
                        var listValue = property.GetValue(loadedItem);

                        var newList = (IList)Activator.CreateInstance(property.PropertyType);

                        if (listValue is IList list)
                        {
                            foreach (var item in list)
                            {
                                _ = newList.Add(item);
                            }
                        }

                        property.SetValue(targetItem, newList);
                    }
                    else if (property.PropertyType.IsNullable())
                    {
                        var actualValue = property.GetValue(loadedItem);

                        property.SetValue(targetItem, actualValue);
                    }
                }
            }
            return targetItem;
        }

        protected virtual void CheckItems<TItem>(List<TItem> loadedItems, List<TItem> targetItems, DefinitionCache<TItem> referenceCache) where TItem : GameFrame.Core.Definitions.BaseDefinition, new()
        {
            if (loadedItems?.Count > 0)
            {
                foreach (var loadedItem in loadedItems)
                {
                    var targetItem = CheckItem(loadedItem, referenceCache);

                    targetItems.Add(targetItem);
                }
            }
        }
    }
}
