using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Core.Model
{
    public class Market
    {
        public List<MineralMarketValue> MineralValues { get; set; }
        public Double UpdateInterval { get; set; }
        public Double Volatility { get; set; }
        public Boolean EnableRandomEvents { get; set; }
        public Double EventProbability { get; set; }
        public Double EventImpactMultiplier { get; set; }
        public List<MarketEvent> Events { get; set; }

    }
}
