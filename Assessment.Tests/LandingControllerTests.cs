using Microsoft.AspNetCore.Mvc;
using Assessment.Controllers;
using Xunit;

namespace Assessment.Tests
{
    public class LandingControllerTests
    {
        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            var controller = new LandingController();

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result); // Check if the result is of type ViewResult
        }
    }
}