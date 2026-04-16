using LandscapeDesign.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandscapeDesign.Core.Models
{
    /// <summary>
    /// Содержит все входные параметры для оценки стоимости ландшафтного проекта.
    /// </summary>
    public class LandscapeProjectRequest
    {
        public string PlotType { get; set; }                // "Lawn", "Garden", "Park", "Roof"
        public double Area { get; set; }                    // 10.0 - 5000.0
        public SoilComplexity SoilComplexity { get; set; } // Simple, Medium, Complex
        public string Style { get; set; }                  // "Modern", "Classic", "Japanese", "Rustic"
        public bool RequiresIrrigation { get; set; }
        public List<string> PlantCategories { get; set; }  // "Trees", "Shrubs", "Flowers", "Conifers"
        public string Season { get; set; }                // "Spring", "Summer", "Autumn", "Winter"
    }
}
