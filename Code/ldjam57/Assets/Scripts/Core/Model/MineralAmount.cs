using System;

namespace Assets.Scripts.Core.Model
{
    public class MineralAmount
    {
        public MineralAmount(Mineral mineral, Double amount = 0)
        {
            this.Mineral = mineral;
            this.Amount = amount;
        }

        public Mineral Mineral { get; }

        public Double Amount { get; set; }
    }
}
