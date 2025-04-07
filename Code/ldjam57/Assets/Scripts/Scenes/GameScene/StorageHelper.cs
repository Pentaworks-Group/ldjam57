
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
            List<Mineral> minerals;
            if (selectedMinerals == null)
            {
                minerals = storage.Keys.ToList();
            }
            else
            {
                minerals = selectedMinerals.ToList();
            }
            foreach (var mineral in minerals)
            {
                if (storage.TryGetValue(mineral, out double stored))
                {
                    usedCapacity += stored * mineral.Weight;
                }
                else
                {
                    selectedMinerals?.Remove(mineral);
                }
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
            double storedAmount = GetStoredAmount(from, allowedMaterials);
            double amountMoved = 0;
            if (amountToMove < storedAmount)
            {
                var toStorage = to.GetStorage();
                var percentage = amountToMove / storedAmount;
                percentage = Math.Min(percentage, 1);
                foreach (var mat in allowedMaterials)
                {
                    var value = from.GetStorage()[mat];
                    double transferedAmount = value * percentage; ;
                    if (toStorage.ContainsKey(mat))
                    {
                        toStorage[mat] += transferedAmount;
                        from.GetStorage()[mat] -= transferedAmount;
                    }
                    else if (to.AllowsNewTypes())
                    {
                        toStorage[mat] = transferedAmount;
                        from.GetStorage()[mat] -= transferedAmount;
                    }
                    amountMoved += transferedAmount * mat.Weight;
                }
            }
            else
            {
                var toStorage = to.GetStorage();
                foreach (var mat in allowedMaterials)
                {
                    var transferedAmount = from.GetStorage()[mat];
                    if (toStorage.ContainsKey(mat))
                    {
                        toStorage[mat] += transferedAmount;
                        from.GetStorage()[mat] = 0;
                    }
                    else if (to.AllowsNewTypes())
                    {
                        toStorage[mat] = transferedAmount;
                        from.GetStorage()[mat] = 0;
                    }
                    amountMoved += transferedAmount * mat.Weight;
                }
            }
            to.StorageChanged();
            from.StorageChanged();
            return amountMoved;
        }
    }
}