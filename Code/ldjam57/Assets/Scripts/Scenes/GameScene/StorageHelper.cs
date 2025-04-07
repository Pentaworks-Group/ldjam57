
using Assets.Scripts.Core.Model;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Assets.Scripts.Scenes.GameScene
{
    public class StorageHelper
    {
        public static double GetStoredAmount(Dictionary<Mineral, Double> storage, List<Mineral> selectedMinerals = null)
        {
            double usedCapacity = 0;
            selectedMinerals ??= storage.Keys.ToList();
            foreach (var mineral in selectedMinerals)
            {
                usedCapacity += storage[mineral] * mineral.Weight;
            }
            return usedCapacity;
        }

        public static double GetStoredAmount(IStorage storage, List<Mineral> selectedMinerals = null)
        {
            return GetStoredAmount(storage.GetStorage(), selectedMinerals);
        }

        public static double MoveStuff(IStorage from, IStorage to, double amountToMove)
        {
            double storedAmountTo = GetStoredAmount(to);
            double freeSpace = to.GetCapacity() - storedAmountTo;
            if (freeSpace <= 0.0001)
            {
                return 0;
            }
            amountToMove = Math.Min(freeSpace, amountToMove);
            List<Mineral> allowedMaterials = new List<Mineral>();
            if (to.AllowsNewTypes())
            {
                foreach (var pair in from.GetStorage())
                {
                    if (pair.Value > 0)
                    {
                        allowedMaterials.Add(pair.Key);
                    }
                }
            }
            else
            {
                foreach (var pair in to.GetStorage())
                {
                    allowedMaterials.Add(pair.Key);
                }
            }
            if (allowedMaterials.Count < 1)
            {
                return 0;
            }
            double storedAmount = GetStoredAmount(from);
            if (amountToMove < storedAmount)
            {
                var toStorage = to.GetStorage();
                var percentage = amountToMove / storedAmount;
                percentage = Math.Min(percentage, 1);
                double amountMoved = 0;
                foreach (var pair in from.GetStorage().ToList())
                {
                    double transferedAmount = pair.Value * percentage; ;
                    if (toStorage.ContainsKey(pair.Key))
                    {
                        toStorage[pair.Key] += transferedAmount;
                        from.GetStorage()[pair.Key] -= transferedAmount;
                    }
                    else if (to.AllowsNewTypes())
                    {
                        toStorage[pair.Key] = transferedAmount;
                        from.GetStorage()[pair.Key] -= transferedAmount;
                    }
                    amountMoved += transferedAmount * pair.Key.Weight;
                }
                to.StorageChanged();
                from.StorageChanged();
                return amountMoved;
            }
            else
            {
                double amountMoved = 0;
                foreach (var pair in from.GetStorage().ToList())
                {
                    var toStorage = to.GetStorage();
                    double transferedAmount = pair.Value;
                    if (toStorage.ContainsKey(pair.Key))
                    {
                        toStorage[pair.Key] += pair.Value;
                        from.GetStorage()[pair.Key] = 0;
                    }
                    else if (to.AllowsNewTypes())
                    {
                        toStorage[pair.Key] = pair.Value;
                        from.GetStorage()[pair.Key] = 0;
                    }
                    amountMoved += transferedAmount * pair.Key.Weight;
                }
                to.StorageChanged();
                from.StorageChanged();
                return amountMoved;
            }
        }
    }
}