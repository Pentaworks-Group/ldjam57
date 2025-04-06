
using Assets.Scripts.Core.Model;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Assets.Scripts.Scenes.GameScene
{
    public class StorageHelper
    {
        public static double GetStoredAmount(Dictionary<Mineral, Double> storage)
        {
            double usedCapacity = 0;
            foreach (var pair in storage)
            {
                usedCapacity += pair.Value * pair.Key.Weight;
            }
            return usedCapacity;
        }

        public static double GetStoredAmount(IStorage storage)
        {
            return GetStoredAmount(storage.GetStorage());
        }

        public static double MoveStuff(IStorage from, IStorage to, double amountToMove)
        {
            double storedAmount = StorageHelper.GetStoredAmount(from);
            if (amountToMove < storedAmount)
            {
                var toStorage = to.GetStorage();
                var percentage = amountToMove / storedAmount;
                percentage = Math.Min(percentage, 1);
                foreach (var pair in from.GetStorage().ToList())
                {
                    if (toStorage.ContainsKey(pair.Key))
                    {
                        var transferedAmount = pair.Value * percentage;
                        toStorage[pair.Key] += transferedAmount;
                        from.GetStorage()[pair.Key] -= transferedAmount;
                    }
                    else
                    {
                        var transferedAmount = pair.Value * percentage;
                        toStorage[pair.Key] = transferedAmount;
                        from.GetStorage()[pair.Key] -= transferedAmount;
                    }
                }
                return amountToMove;
            }
            foreach (var pair in from.GetStorage().ToList())
            {
                var storage = to.GetStorage();
                if (storage.ContainsKey(pair.Key))
                {
                    storage[pair.Key] += pair.Value;
                    from.GetStorage().Remove(pair.Key);
                }
                else
                {
                    storage[pair.Key] = pair.Value;
                    from.GetStorage().Remove(pair.Key);
                }
            }
            return amountToMove - storedAmount;
        }
    }
}