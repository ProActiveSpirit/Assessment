using Microsoft.EntityFrameworkCore;
using Assessment.Data;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace Assessment.Tests
{
    public class ApplicationDbContextTests
    {
        private ApplicationDbContext _context = null!; // Null-forgiving operator applied

        public void Setup()
        {
            // Use an in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // In-memory DB setup
                .Options;

            _context = new ApplicationDbContext(options);
        }

        public void Teardown()
        {
            // Dispose of the DbContext after each test
            _context.Dispose();
        }

        [Fact]
        public void CanAddUserToDatabase()
        {
            // Arrange
            Setup(); // Call setup explicitly since XUnit does not have [SetUp]
            var user = new IdentityUser
            {
                UserName = "testuser",
                Email = "testuser@example.com"
            };

            // Act
            _context.Users.Add(user);
            _context.SaveChanges();

            // Assert
            Assert.Equal(1, _context.Users.Count()); // Ensure the user was added
            Teardown(); // Call teardown explicitly
        }
    }
}