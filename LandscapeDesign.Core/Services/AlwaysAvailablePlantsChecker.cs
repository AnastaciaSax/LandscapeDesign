using LandscapeDesign.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandscapeDesign.Core.Services
{
    /// <summary>
    /// Заглушка – всегда true.
    /// </summary>
    public class AlwaysAvailablePlantsChecker : IPlantAvailabilityChecker
    {
        public bool ArePlantsAvailable(List<string> plantCategories, int seasonCode)
        {
            return true;
        }
    }
}
