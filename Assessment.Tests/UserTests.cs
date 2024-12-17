using Assessment.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assessment.Tests
{
    public class UserControllerTests
    {
        private UserController _controller;
        private Mock<UserManager<IdentityUser>> _mockUserManager;

        public UserControllerTests()
        {
            // Mock dependencies for UserManager
            var store = new Mock<IUserStore<IdentityUser>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var passwordHasher = new Mock<IPasswordHasher<IdentityUser>>();
            var userValidators = new List<IUserValidator<IdentityUser>> { new Mock<IUserValidator<IdentityUser>>().Object };
            var passwordValidators = new List<IPasswordValidator<IdentityUser>> { new Mock<IPasswordValidator<IdentityUser>>().Object };
            var keyNormalizer = new Mock<ILookupNormalizer>();
            var errors = new Mock<IdentityErrorDescriber>();
            var services = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<UserManager<IdentityUser>>>();

            // Initialize the mocked UserManager
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                store.Object,
                options.Object,
                passwordHasher.Object,
                userValidators,
                passwordValidators,
                keyNormalizer.Object,
                errors.Object,
                services.Object,
                logger.Object
            );

            // Pass the mocked UserManager to the controller
            _controller = new UserController(_mockUserManager.Object);
        }
        [Fact]
        public async Task GetAll_ReturnsUsers()
        {
            // Arrange
            var users = new List<IdentityUser>
            {
                new IdentityUser { UserName = "user1", Email = "user1@example.com" },
                new IdentityUser { UserName = "user2", Email = "user2@example.com" }
            };

            IQueryable<IdentityUser> queryUsers = users.AsQueryable();

            _mockUserManager.Setup(m => m.Users).Returns(queryUsers);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);

            var returnedUsers = okResult.Value as IEnumerable<IdentityUser>;
            Assert.NotNull(returnedUsers); // Ensure returnedUsers is not null
            Assert.Equal(2, returnedUsers?.Count());
        }

        [Fact]
        public async Task GetByEmail_ReturnsUser_WhenExists()
        {
            // Arrange
            var user = new IdentityUser { UserName = "user1", Email = "user1@example.com" };
            _mockUserManager.Setup(m => m.FindByEmailAsync("user1@example.com"))
                            .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserByEmail("user1@example.com");

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);

            var returnedUser = okResult.Value as IdentityUser;
            Assert.NotNull(returnedUser); // Ensure returnedUser is not null
            Assert.Equal("user1", returnedUser?.UserName);
        }

        [Fact]
        public async Task GetByEmail_ReturnsNotFound_WhenNotExists()
        {
            // Arrange
            _mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                            .ReturnsAsync((IdentityUser?)null);

            // Act
            var result = await _controller.GetUserByEmail("nonexistent@example.com");

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenInvalid()
        {
            // Arrange
            var user = new IdentityUser { UserName = "user1", Email = "user1@example.com" };
            var identityErrors = new List<IdentityError>
            {
                new IdentityError { Description = "Password is too weak" }
            };
            var failedResult = IdentityResult.Failed(identityErrors.ToArray());

            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                            .ReturnsAsync(failedResult);

            // Act
            var result = await _controller.CreateUser(user);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);

            var modelState = badRequestResult.Value as SerializableError;
            Assert.NotNull(modelState); // Ensure modelState is not null
            Assert.True(modelState?.ContainsKey(string.Empty));
        }
    }
}