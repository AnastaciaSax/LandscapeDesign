using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandscapeDesign.Core.Interfaces
{
    /// <summary>
    /// Внешний сервис по ценообразованию на индивидуальные ирригационные системы.
    /// </summary>
    public interface IIrrigationQuoteService
    {
        decimal GetCustomIrrigationQuote(double area, string plotType);
    }
}
