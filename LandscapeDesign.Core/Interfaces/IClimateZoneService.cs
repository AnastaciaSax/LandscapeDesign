using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandscapeDesign.Core.Interfaces
{
    /// <summary>
    /// Дает климатическую зону для данного региона
    /// </summary>
    public interface IClimateZoneService
    {
        string GetClimateZone(string region);
    }
}
