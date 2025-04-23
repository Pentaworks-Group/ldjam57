
using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Core.Model;

namespace Assets.Scripts.Scenes.GameScene
{
    public class StorageHelper
    {
        public static double GetStoredAmount(Dictionary<String, MineralAmount> storage, List<Mineral> selectedMinerals = null)
        {
            double usedCapacity = 0;

            List<Mineral> minerals;

            if (selectedMinerals == null)
            {
                minerals = storage.Values.Select(m => m.Mineral).ToList();
            }
            else
            {
                minerals = selectedMinerals.ToList();
            }

            foreach (var mineral in minerals)
            {
                if (storage.TryGetValue(mineral.Reference, out var storedAmount))
                {
                    usedCapacity += storedAmount.Amount * mineral.Weight;
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
                    if (pair.Value.Amount > 0)
                    {
                        allowedMaterials.Add(pair.Value.Mineral);
                    }
                }
            }
            else
            {
                foreach (var pair in to.GetStorage())
                {
                    allowedMaterials.Add(pair.Value.Mineral);
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
                    var mineralAmount = from.GetStorage()[mat.Reference];

                    double transferedAmount = mineralAmount.Amount * percentage;
                    if (toStorage.ContainsKey(mat.Reference))
                    {
                        toStorage[mat.Reference].Amount += transferedAmount;
                        from.GetStorage()[mat.Reference].Amount -= transferedAmount;
                    }
                    else if (to.AllowsNewTypes())
                    {
                        toStorage[mat.Reference] = new MineralAmount(mat, transferedAmount);
                        from.GetStorage()[mat.Reference].Amount -= transferedAmount;
                    }
                    amountMoved += transferedAmount * mat.Weight;
                }
            }
            else
            {
                var toStorage = to.GetStorage();

                foreach (var mat in allowedMaterials)
                {
                    var transferedAmount = from.GetStorage()[mat.Reference];

                    if (toStorage.ContainsKey(mat.Reference))
                    {
                        toStorage[mat.Reference].Amount += transferedAmount.Amount;
                        from.GetStorage()[mat.Reference].Amount = 0;
                    }
                    else if (to.AllowsNewTypes())
                    {
                        toStorage[mat.Reference] = new MineralAmount(mat, transferedAmount.Amount);
                        from.GetStorage()[mat.Reference].Amount = 0;
                    }

                    amountMoved += transferedAmount.Amount * mat.Weight;
                }
            }

            to.StorageChanged();
            from.StorageChanged();
            return amountMoved;
        }
    }
}