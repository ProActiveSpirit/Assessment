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
            var controller = new LandingController();

            var result = controller.Index();

            Assert.IsType<ViewResult>(result); // Check if the result is of type ViewResult
        }
    }
}