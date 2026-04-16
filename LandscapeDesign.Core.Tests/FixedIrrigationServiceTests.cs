using LandscapeDesign.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
// TDD component tests (Red-Green-Refactor demonstrated separately)

namespace LandscapeDesign.Core.Tests
{
    public class FixedIrrigationServiceTests
    {
        private readonly FixedIrrigationService _sut = new();

        [Theory]
        [InlineData(50, 15000)]   // 50 * 300
        [InlineData(99, 29700)]   // 99 * 300
        [InlineData(100, 30000)]  // 100 * 300  (branch: area < 100 false)
        [InlineData(101, 30250)]  // 30000 + 1*250
        [InlineData(200, 55000)]  // 30000 + 100*250
        [InlineData(0, 0)]
        public void GetCustomIrrigationQuote_ValidArea_ReturnsCorrectCost(double area, decimal expected)
        {
            // Act
            var result = _sut.GetCustomIrrigationQuote(area, "Lawn");

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetCustomIrrigationQuote_NegativeArea_ThrowsArgumentOutOfRangeException()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => _sut.GetCustomIrrigationQuote(-10, "Lawn"));
        }
    }
}
