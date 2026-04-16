using LandscapeDesign.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandscapeDesign.Core.Services
{
    /// <summary>
    /// Заглушка – всегда "Temperate".
    /// </summary>
    public class FixedClimateZoneService : IClimateZoneService
    {
        public string GetClimateZone(string region)
        {
            return "Temperate";
        }
    }
}
