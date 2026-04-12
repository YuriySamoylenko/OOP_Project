using Microsoft.EntityFrameworkCore;
using Moq;
using Pms.Core.Entities;
using Pms.Dal;
using Pms.Data;

namespace Pms.Tests
{
    [TestClass]
    public sealed class RepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> _options;
        private Mock<IDbContextFactory<ApplicationDbContext>> _factoryMock;
        private Repository<Project> _repository;
        private Microsoft.Data.Sqlite.SqliteConnection _connection;

        [TestInitialize]
        public void Setup()
        {
            _connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;

            using (var context = new ApplicationDbContext(_options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
            }

            _factoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            _factoryMock.Setup(f => f.CreateDbContextAsync(default))
                .Returns(() => Task.FromResult(new ApplicationDbContext(_options)));

            _repository = new Repository<Project>(_factoryMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _connection.Close(); // Тепер база видалиться після тесту
        }

        [TestMethod]
        public async Task CreateAsync_ShouldAddItemToDatabase()
        {
            // Arrange
            var entity = new Project { Name = "New Entity", Code = "NE" };

            // Act
            await _repository.CreateAsync(entity);

            // Assert
            using var context = new ApplicationDbContext(_options);
            var result = await context.Set<Project>().FirstOrDefaultAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual("New Entity", result.Name);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnAllItems()
        {
            // Arrange
            using (var context = new ApplicationDbContext(_options))
            {
                context.Set<Project>().AddRange(
                    new Project { Name = "E1", Code = "NE" },
                    new Project { Name = "E2", Code = "NE" }
                );
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnCorrectItem()
        {
            // Arrange
            int savedId;
            using (var context = new ApplicationDbContext(_options))
            {
                var entity = new Project { Name = "Target", Code = "NE" };
                context.Set<Project>().Add(entity);
                await context.SaveChangesAsync();
                savedId = entity.Id;
            }

            // Act
            var result = await _repository.GetByIdAsync(savedId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Target", result.Name);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldModifyExistingData()
        {
            // Arrange
            int savedId;
            using (var context = new ApplicationDbContext(_options))
            {
                var entity = new Project { Name = "Old Name", Code = "NE" };
                context.Set<Project>().Add(entity);
                await context.SaveChangesAsync();
                savedId = entity.Id;
            }

            var updatedEntity = new Project { Id = savedId, Name = "Updated Name", Code = "NE" };

            // Act
            await _repository.UpdateAsync(updatedEntity);

            // Assert
            using (var context = new ApplicationDbContext(_options))
            {
                var result = await context.Set<Project>().FindAsync(savedId);
                Assert.AreEqual("Updated Name", result.Name);
            }
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldRemoveItemFromDatabase()
        {
            // Arrange
            int savedId;
            using (var context = new ApplicationDbContext(_options))
            {
                var entity = new Project { Name = "To Delete", Code = "NE" };
                context.Set<Project>().Add(entity);
                await context.SaveChangesAsync();
                savedId = entity.Id;
            }

            // Act
            await _repository.DeleteAsync(savedId);

            // Assert
            using (var context = new ApplicationDbContext(_options))
            {
                var result = await context.Set<Project>().FindAsync(savedId);
                Assert.IsNull(result, "Entity should be deleted from DB");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task CreateAsync_NullItem_ShouldThrowException()
        {
            // Act
            await _repository.CreateAsync(null!);
        }
    }
}
