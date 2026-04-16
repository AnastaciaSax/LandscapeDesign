using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandscapeDesign.Core.Models
{
    /// <summary>
    /// Результат оценки с разбивкой по пунктам.
    /// </summary>
    public class LandscapeQuote
    {
        public double WorkCost { get; set; }
        public double GreeneryCost { get; set; }
        public double IrrigationCost { get; set; }
        public double SeasonalFactor { get; set; }
        public double TotalCost { get; set; }
    }
}
