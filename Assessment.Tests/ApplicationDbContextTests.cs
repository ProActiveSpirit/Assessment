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
            _context.Dispose();
        }

        [Fact]
        public void CanAddUserToDatabase()
        {
            Setup(); // Call setup explicitly since XUnit does not have [SetUp]
            var user = new IdentityUser
            {
                UserName = "testuser",
                Email = "testuser@example.com"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            Assert.Equal(1, _context.Users.Count());
            Teardown(); // Call teardown explicitly
        }
    }
}