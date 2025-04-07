using System;

namespace Assets.Scripts.Core.Definitons
{
    public class MineralValueDefinition
    {
        public MineralDefinition Mineral { get; set; }
        public Double? Value { get; set; }
        public Double? Volatility { get; set; }
        public Double? MinPrice { get; set; }
        public Double? MaxPrice { get; set; }
        public Double? TrendStrength { get; set;}
    }
}
