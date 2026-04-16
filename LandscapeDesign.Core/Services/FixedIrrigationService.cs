// TDD-Компонент с кусочно-линейной системой ценообразования.
using LandscapeDesign.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandscapeDesign.Core.Services
{
    /// <summary>
    /// Фиксированная ​​реализация сервиса расчета стоимости орошения.
    /// Используется кусочно-линейная формула:
    /// - площадь < 100 : площадь * 300
    /// - площадь >= 100 : 100 * 300 + (площадь - 100) * 250
    /// </summary>
    public class FixedIrrigationService : IIrrigationQuoteService
    {
        public decimal GetCustomIrrigationQuote(double area, string plotType)
        {
            // plotType ignored – fixed formula only depends on area
            if (area < 0)
                throw new ArgumentOutOfRangeException(nameof(area), "Area cannot be negative.");

            double cost;
            if (area < 100)
                cost = area * 300;
            else
                cost = 100 * 300 + (area - 100) * 250;

            return (decimal)cost;
        }
    }
}
