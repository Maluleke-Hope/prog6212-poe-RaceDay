using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.Models;
using Xunit;

namespace RaceDay.Tests
{
    public class EventsControllerTests
    {
        private RaceDayDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new RaceDayDbContext(options);
            databaseContext.Database.EnsureCreated();
            return databaseContext;
        }

        [Fact]
        public async Task PostEvent_ReturnsUnauthorized_WhenUserIsNotOrganiser()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new EventsController(dbContext);

            // Mock HttpContext session with a Participant role
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new FakeSession();
            httpContext.Session.SetString("UserRole", "Participant");
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            var newEvent = new Event
            {
                Title = "Test Marathon",
                Description = "Test Description",
                Date = DateTime.Now.AddDays(5),
                Location = "Joburg",
                Distance = 10,
                EventType = "Running",
                OrganiserId = 1
            };

            // Act
            var result = await controller.PostEvent(newEvent);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }

        [Fact]
        public async Task PostEvent_ReturnsCreated_WhenUserIsOrganiser()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new EventsController(dbContext);

            // Mock HttpContext session with an Organiser role
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new FakeSession();
            httpContext.Session.SetString("UserRole", "Organiser");
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            var newEvent = new Event
            {
                Title = "Championship Trail Run",
                Description = "Organiser managed event",
                Date = DateTime.Now.AddDays(10),
                Location = "Pretoria",
                Distance = 21.1,
                EventType = "Running",
                OrganiserId = 1
            };

            // Act
            var result = await controller.PostEvent(newEvent);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(201, createdAtActionResult.StatusCode);

            var createdEvent = Assert.IsType<Event>(createdAtActionResult.Value);
            Assert.Equal("Championship Trail Run", createdEvent.Title);
        }
    }

    // Helper fake session class for testing session dependent controllers
    public class FakeSession : ISession
    {
        private readonly Dictionary<string, byte[]> _sessionStorage = new();
        public bool IsAvailable => true;
        public string Id => Guid.NewGuid().ToString();
        IEnumerable<string> ISession.Keys => _sessionStorage.Keys;
        public void Clear() => _sessionStorage.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _sessionStorage.Remove(key);
        public void Set(string key, byte[] value) => _sessionStorage[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);
    }
}