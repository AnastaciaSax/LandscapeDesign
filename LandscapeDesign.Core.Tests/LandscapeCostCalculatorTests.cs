using LandscapeDesign.Core;
using LandscapeDesign.Core.Enums;
using LandscapeDesign.Core.Interfaces;
using LandscapeDesign.Core.Models;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace LandscapeDesign.Core.Tests
{
    public class LandscapeCostCalculatorTests
    {
        private readonly Mock<IIrrigationQuoteService> _irrigationMock;
        private readonly Mock<IPlantAvailabilityChecker> _plantCheckerMock;
        private readonly Mock<IClimateZoneService> _climateZoneMock;
        private readonly LandscapeCostCalculator _sut;

        public LandscapeCostCalculatorTests()
        {
            _irrigationMock = new Mock<IIrrigationQuoteService>();
            _plantCheckerMock = new Mock<IPlantAvailabilityChecker>();
            _climateZoneMock = new Mock<IClimateZoneService>();

            _sut = new LandscapeCostCalculator(
                _irrigationMock.Object,
                _plantCheckerMock.Object,
                _climateZoneMock.Object);
        }

        [Fact]
        public void CalculateEstimate_ValidRequest_ReturnsCorrectWorkCost()
        {
            // Arrange
            var request = new LandscapeProjectRequest
            {
                PlotType = "Lawn",
                Area = 100,
                SoilComplexity = SoilComplexity.Simple,
                Style = "Modern",
                RequiresIrrigation = false,
                PlantCategories = new List<string>(),
                Season = "Summer"
            };

            // Act
            var result = _sut.CalculateEstimate(request);

            // Assert
            Assert.Equal(60000, result.WorkCost, 2);
            Assert.Equal(0, result.GreeneryCost);
            Assert.Equal(0, result.IrrigationCost);
            Assert.Equal(1.0, result.SeasonalFactor);
            Assert.Equal(60000, result.TotalCost, 2);
        }

        [Theory]
        [InlineData("Garden", 200, SoilComplexity.Medium, "Classic", 800 * 200 * 1.3 * 1.0)] // 208000
        [InlineData("Park", 10, SoilComplexity.Simple, "Rustic", 650 * 10 * 1.0 * 0.9)]      // 5850
        [InlineData("Roof", 5000, SoilComplexity.Simple, "Modern", 1200 * 5000 * 1.0 * 1.2)] // 7,200,000
        public void CalculateEstimate_VariousValidInputs_ComputesWorkCorrectly(
            string plotType, double area, SoilComplexity soil, string style, double expectedWork)
        {
            // Arrange
            var request = new LandscapeProjectRequest
            {
                PlotType = plotType,
                Area = area,
                SoilComplexity = soil,
                Style = style,
                RequiresIrrigation = false,
                PlantCategories = new List<string>(),
                Season = "Summer"
            };

            // Act
            var result = _sut.CalculateEstimate(request);

            // Assert
            Assert.Equal(expectedWork, result.WorkCost, 2);
        }

        [Theory]
        [InlineData(9.99)]
        [InlineData(5000.01)]
        public void CalculateEstimate_AreaOutOfRange_ThrowsArgumentOutOfRangeException(double area)
        {
            // Arrange
            var request = new LandscapeProjectRequest
            {
                PlotType = "Lawn",
                Area = area,
                SoilComplexity = SoilComplexity.Simple,
                Style = "Modern",
                RequiresIrrigation = false,
                PlantCategories = new List<string>(),
                Season = "Summer"
            };

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => _sut.CalculateEstimate(request));
        }

        [Theory]
        [InlineData("Forest")]
        [InlineData("")]
        [InlineData(null)]
        public void CalculateEstimate_InvalidPlotType_ThrowsArgumentException(string plotType)
        {
            // Arrange
            var request = new LandscapeProjectRequest
            {
                PlotType = plotType,
                Area = 100,
                SoilComplexity = SoilComplexity.Simple,
                Style = "Modern",
                RequiresIrrigation = false,
                PlantCategories = new List<string>(),
                Season = "Summer"
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _sut.CalculateEstimate(request));
        }

        [Theory]
        [InlineData("Baroque")]
        [InlineData("")]
        [InlineData(null)]
        public void CalculateEstimate_InvalidStyle_ThrowsArgumentException(string style)
        {
            // Arrange
            var request = new LandscapeProjectRequest
            {
                PlotType = "Lawn",
                Area = 100,
                SoilComplexity = SoilComplexity.Simple,
                Style = style,
                RequiresIrrigation = false,
                PlantCategories = new List<string>(),
                Season = "Summer"
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _sut.CalculateEstimate(request));
        }

        [Theory]
        [InlineData("Monsoon")]
        [InlineData("")]
        [InlineData(null)]
        public void CalculateEstimate_InvalidSeason_ThrowsArgumentException(string season)
        {
            // Arrange
            var request = new LandscapeProjectRequest
            {
                PlotType = "Lawn",
                Area = 100,
                SoilComplexity = SoilComplexity.Simple,
                Style = "Modern",
                RequiresIrrigation = false,
                PlantCategories = new List<string>(),
                Season = season
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _sut.CalculateEstimate(request));
        }

        [Fact]
        public void CalculateEstimate_JapaneseStyle_AppliesCoefficient1_8()
        {
            // Arrange
            var request = new LandscapeProjectRequest
            {
                PlotType = "Lawn",
                Area = 100,
                SoilComplexity = SoilComplexity.Simple,
                Style = "Japanese",
                RequiresIrrigation = false,
                PlantCategories = new List<string>(),
                Season = "Summer"
            };
            double expectedWork = 500 * 100 * 1.0 * 1.8; // 90000

            // Act
            var result = _sut.CalculateEstimate(request);

            // Assert
            Assert.Equal(expectedWork, result.WorkCost, 2);
        }

        [Fact]
        public void CalculateEstimate_WinterSeason_AppliesFactor1_4()
        {
            // Arrange
            var request = new LandscapeProjectRequest
            {
                PlotType = "Lawn",
                Area = 100,
                SoilComplexity = SoilComplexity.Simple,
                Style = "Modern",
                RequiresIrrigation = false,
                PlantCategories = new List<string>(),
                Season = "Winter"
            };
            double expectedTotal = 60000 * 1.4; // 84000

            // Act
            var result = _sut.CalculateEstimate(request);

            // Assert
            Assert.Equal(expectedTotal, result.TotalCost, 2);
        }

        [Fact]
        public void CalculateEstimate_OnePlantCategory_AddsCorrectCost()
        {
            // Arrange
            var request = new LandscapeProjectRequest
            {
                PlotType = "Lawn",
                Area = 100,
                SoilComplexity = SoilComplexity.Simple,
                Style = "Modern",
                RequiresIrrigation = false,
                PlantCategories = new List<string> { "Trees" },
                Season = "Summer"
            };
            double expectedGreenery = 100 * 200; // 20000
            double expectedTotal = 60000 + expectedGreenery; // 80000

            // Act
            var result = _sut.CalculateEstimate(request);

            // Assert
            Assert.Equal(expectedGreenery, result.GreeneryCost, 2);
            Assert.Equal(expectedTotal, result.TotalCost, 2);
        }

        [Fact]
        public void CalculateEstimate_MultiplePlantCategories_SumsCosts()
        {
            // Arrange
            var request = new LandscapeProjectRequest
            {
                PlotType = "Lawn",
                Area = 100,
                SoilComplexity = SoilComplexity.Simple,
                Style = "Modern",
                RequiresIrrigation = false,
                PlantCategories = new List<string> { "Trees", "Shrubs", "Flowers" },
                Season = "Summer"
            };
            double expectedGreenery = 100 * (200 + 150 + 100); // 45000
            double expectedTotal = 60000 + expectedGreenery;   // 105000

            // Act
            var result = _sut.CalculateEstimate(request);

            // Assert
            Assert.Equal(expectedGreenery, result.GreeneryCost, 2);
            Assert.Equal(expectedTotal, result.TotalCost, 2);
        }

        [Fact]
        public void CalculateEstimate_DuplicatePlantCategories_AddsEach()
        {
            // Arrange
            var request = new LandscapeProjectRequest
            {
                PlotType = "Lawn",
                Area = 100,
                SoilComplexity = SoilComplexity.Simple,
                Style = "Modern",
                RequiresIrrigation = false,
                PlantCategories = new List<string> { "Trees", "Trees" },
                Season = "Summer"
            };
            double expectedGreenery = 100 * 200 * 2; // 40000
            double expectedTotal = 60000 + 40000;    // 100000

            // Act
            var result = _sut.CalculateEstimate(request);

            // Assert
            Assert.Equal(expectedGreenery, result.GreeneryCost, 2);
            Assert.Equal(expectedTotal, result.TotalCost, 2);
        }

        [Fact]
        public void CalculateEstimate_EmptyPlantCategories_ZeroGreeneryCost()
        {
            // Arrange
            var request = new LandscapeProjectRequest
            {
                PlotType = "Lawn",
                Area = 100,
                SoilComplexity = SoilComplexity.Simple,
                Style = "Modern",
                RequiresIrrigation = false,
                PlantCategories = new List<string>(),
                Season = "Summer"
            };

            // Act
            var result = _sut.CalculateEstimate(request);

            // Assert
            Assert.Equal(0, result.GreeneryCost);
        }

        [Fact]
        public void CalculateEstimate_NullPlantCategories_TreatedAsEmpty()
        {
            // Arrange
            var request = new LandscapeProjectRequest
            {
                PlotType = "Lawn",
                Area = 100,
                SoilComplexity = SoilComplexity.Simple,
                Style = "Modern",
                RequiresIrrigation = false,
                PlantCategories = null,
                Season = "Summer"
            };

            // Act
            var result = _sut.CalculateEstimate(request);

            // Assert
            Assert.Equal(0, result.GreeneryCost);
        }

        [Fact]
        public void CalculateEstimate_UnknownPlantCategory_ThrowsArgumentException()
        {
            // Arrange
            var request = new LandscapeProjectRequest
            {
                PlotType = "Lawn",
                Area = 100,
                SoilComplexity = SoilComplexity.Simple,
                Style = "Modern",
                RequiresIrrigation = false,
                PlantCategories = new List<string> { "Bamboo" },
                Season = "Summer"
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _sut.CalculateEstimate(request));
        }

        [Fact]
        public void CalculateEstimate_IrrigationRequired_CallsServiceAndUsesQuote()
        {
            // Arrange
            var request = new LandscapeProjectRequest
            {
                PlotType = "Lawn",
                Area = 150,
                SoilComplexity = SoilComplexity.Simple,
                Style = "Modern",
                RequiresIrrigation = true,
                PlantCategories = new List<string>(),
                Season = "Summer"
            };
            decimal fakeQuote = 50000;
            _irrigationMock.Setup(x => x.GetCustomIrrigationQuote(150, "Lawn"))
                           .Returns(fakeQuote);

            // Act
            var result = _sut.CalculateEstimate(request);

            // Assert
            double expectedWork = 500 * 150 * 1.0 * 1.2; // 90000
            Assert.Equal(expectedWork, result.WorkCost, 2);
            Assert.Equal((double)fakeQuote, result.IrrigationCost, 2);
            Assert.Equal(expectedWork + (double)fakeQuote, result.TotalCost, 2);
            _irrigationMock.Verify(x => x.GetCustomIrrigationQuote(150, "Lawn"), Times.Once);
        }

        [Fact]
        public void CalculateEstimate_IrrigationNotRequired_ZeroIrrigationCost()
        {
            // Arrange
            var request = new LandscapeProjectRequest
            {
                PlotType = "Lawn",
                Area = 150,
                SoilComplexity = SoilComplexity.Simple,
                Style = "Modern",
                RequiresIrrigation = false,
                PlantCategories = new List<string>(),
                Season = "Summer"
            };

            // Act
            var result = _sut.CalculateEstimate(request);

            // Assert
            Assert.Equal(0, result.IrrigationCost);
            _irrigationMock.Verify(x => x.GetCustomIrrigationQuote(It.IsAny<double>(), It.IsAny<string>()), Times.Never);
        }
        [Fact]
        public void Constructor_NullIrrigationService_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new LandscapeCostCalculator(null, _plantCheckerMock.Object, _climateZoneMock.Object));
        }

        [Fact]
        public void Constructor_NullPlantChecker_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new LandscapeCostCalculator(_irrigationMock.Object, null, _climateZoneMock.Object));
        }

        [Fact]
        public void Constructor_NullClimateZoneService_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new LandscapeCostCalculator(_irrigationMock.Object, _plantCheckerMock.Object, null));
        }

        [Fact]
        public void CalculateEstimate_InvalidSoilComplexity_ThrowsArgumentException()
        {
            var request = new LandscapeProjectRequest
            {
                PlotType = "Lawn",
                Area = 100,
                SoilComplexity = (SoilComplexity)999, // недопустимое значение
                Style = "Modern",
                RequiresIrrigation = false,
                PlantCategories = new List<string>(),
                Season = "Summer"
            };
            Assert.Throws<ArgumentException>(() => _sut.CalculateEstimate(request));
        }

        [Fact]
        public void CalculateEstimate_NullRequest_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.CalculateEstimate(null));
        }
    }
}