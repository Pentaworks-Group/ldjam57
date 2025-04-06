
using Assets.Scripts.Core.Model;
using System.Collections.Generic;
using System;

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
                var percentage = amountToMove / storedAmount;
                foreach (var pair in from.GetStorage())
                {
                    var storage = to.GetStorage();
                    if (storage.ContainsKey(pair.Key))
                    {
                        storage[pair.Key] += pair.Value * percentage;
                    }
                    else
                    {
                        storage[pair.Key] = pair.Value * percentage;
                    }
                }
                return amountToMove;
            }
            foreach (var pair in from.GetStorage())
            {
                var storage = to.GetStorage();
                if (storage.ContainsKey(pair.Key))
                {
                    storage[pair.Key] += pair.Value;
                }
                else
                {
                    storage[pair.Key] = pair.Value;
                }
            }
            return amountToMove - storedAmount;
        }
    }
}