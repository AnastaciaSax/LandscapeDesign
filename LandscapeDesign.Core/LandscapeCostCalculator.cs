using LandscapeDesign.Core.Enums;
using LandscapeDesign.Core.Interfaces;
using LandscapeDesign.Core.Models;

namespace LandscapeDesign.Core
{
    /// <summary>
    /// Основной класс калькулятора. Зависимости внедряются через конструктор.
    /// </summary>
    public class LandscapeCostCalculator
    {
        private readonly IIrrigationQuoteService _irrigationService;
        private readonly IPlantAvailabilityChecker _plantChecker;
        private readonly IClimateZoneService _climateZoneService;

        //  ставки за м^2
        private static readonly Dictionary<string, double> PlotBaseRates = new()
        {
            { "Lawn", 500 },
            { "Garden", 800 },
            { "Park", 650 },
            { "Roof", 1200 }
        };

        // Коэффициенты сложности почвы
        private static readonly Dictionary<SoilComplexity, double> SoilCoefficients = new()
        {
            { SoilComplexity.Simple, 1.0 },
            { SoilComplexity.Medium, 1.3 },
            { SoilComplexity.Complex, 2.0 }
        };

        // Коэффициенты стиля
        private static readonly Dictionary<string, double> StyleCoefficients = new()
        {
            { "Modern", 1.2 },
            { "Classic", 1.0 },
            { "Japanese", 1.8 },
            { "Rustic", 0.9 }
        };

        // Plant category rates
        private static readonly Dictionary<string, double> PlantRates = new()
        {
            { "Trees", 200 },
            { "Shrubs", 150 },
            { "Flowers", 100 },
            { "Conifers", 180 }
        };

        // Seasonal coefficients
        private static readonly Dictionary<string, double> SeasonalCoefficients = new()
        {
            { "Spring", 1.1 },
            { "Summer", 1.0 },
            { "Autumn", 1.05 },
            { "Winter", 1.4 }
        };

        public LandscapeCostCalculator(
            IIrrigationQuoteService irrigationService,
            IPlantAvailabilityChecker plantChecker,
            IClimateZoneService climateZoneService)
        {
            _irrigationService = irrigationService ?? throw new ArgumentNullException(nameof(irrigationService));
            _plantChecker = plantChecker ?? throw new ArgumentNullException(nameof(plantChecker));
            _climateZoneService = climateZoneService ?? throw new ArgumentNullException(nameof(climateZoneService));
        }

        /// <summary>
        /// Main estimation method.
        /// </summary>
        /// <param name="request">Project parameters.</param>
        /// <returns>Detailed quote.</returns>
        public LandscapeQuote CalculateEstimate(LandscapeProjectRequest request)
        {
            ValidateRequest(request);

            // 1. Basic work cost
            double workCost = PlotBaseRates[request.PlotType] *
                              request.Area *
                              SoilCoefficients[request.SoilComplexity] *
                              StyleCoefficients[request.Style];

            // 2. Greenery (plants) cost
            double greeneryCost = 0;
            foreach (var category in request.PlantCategories)
            {
                if (!PlantRates.ContainsKey(category))
                    throw new ArgumentException($"Unknown plant category: {category}");
                greeneryCost += PlantRates[category] * request.Area;
            }

            // 3. Irrigation cost
            double irrigationCost = 0;
            if (request.RequiresIrrigation)
            {
                // External service call
                decimal quote = _irrigationService.GetCustomIrrigationQuote(request.Area, request.PlotType);
                irrigationCost = (double)quote;
            }

            // 4. Seasonal adjustment
            double seasonalFactor = SeasonalCoefficients[request.Season];
            double total = (workCost + greeneryCost + irrigationCost) * seasonalFactor;

            return new LandscapeQuote
            {
                WorkCost = workCost,
                GreeneryCost = greeneryCost,
                IrrigationCost = irrigationCost,
                SeasonalFactor = seasonalFactor,
                TotalCost = total
            };
        }

        private void ValidateRequest(LandscapeProjectRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // PlotType validation
            if (string.IsNullOrEmpty(request.PlotType))
                throw new ArgumentException("Plot type cannot be null or empty.", nameof(request.PlotType));
            if (!PlotBaseRates.ContainsKey(request.PlotType))
                throw new ArgumentException($"Invalid plot type: {request.PlotType}");

            // Area validation
            if (request.Area < 10.0 || request.Area > 5000.0)
                throw new ArgumentOutOfRangeException(nameof(request.Area), "Area must be between 10 and 5000 m².");

            // SoilComplexity validation
            if (!Enum.IsDefined(typeof(SoilComplexity), request.SoilComplexity))
                throw new ArgumentException($"Invalid soil complexity: {request.SoilComplexity}");

            // Style validation
            if (string.IsNullOrEmpty(request.Style))
                throw new ArgumentException("Style cannot be null or empty.", nameof(request.Style));
            if (!StyleCoefficients.ContainsKey(request.Style))
                throw new ArgumentException($"Invalid style: {request.Style}");

            // PlantCategories validation
            if (request.PlantCategories == null)
                request.PlantCategories = new List<string>();

            // Season validation
            if (string.IsNullOrEmpty(request.Season))
                throw new ArgumentException("Season cannot be null or empty.", nameof(request.Season));
            if (!SeasonalCoefficients.ContainsKey(request.Season))
                throw new ArgumentException($"Invalid season: {request.Season}");
        }
    }
}
