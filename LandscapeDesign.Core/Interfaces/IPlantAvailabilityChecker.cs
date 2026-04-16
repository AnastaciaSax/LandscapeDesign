using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandscapeDesign.Core.Interfaces
{
    /// <summary>
    /// Проверяет наличие выбранных категорий растений в питомнике в данный сезон.
    /// </summary>
    public interface IPlantAvailabilityChecker
    {
        bool ArePlantsAvailable(List<string> plantCategories, int seasonCode);
    }
}
