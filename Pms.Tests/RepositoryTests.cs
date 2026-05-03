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

        [TestMethod]
        public async Task GetAllAsync_WithPredicate_ShouldReturnFilteredItems()
        {
            // Arrange
            using (var context = new ApplicationDbContext(_options))
            {
                context.Set<Project>().AddRange(
                    new Project { Name = "Alpha", Code = "A" },
                    new Project { Name = "Beta", Code = "B" },
                    new Project { Name = "Gamma", Code = "G" }
                );
                await context.SaveChangesAsync();
            }

            // Act - фільтруємо лише ті, що починаються на 'G'
            var result = await _repository.GetAllAsync(p => p.Name.StartsWith("G"));

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Gamma", result.First().Name);
        }

        [TestMethod]
        public async Task GetByConditionAsync_ShouldReturnFirstMatchingItem()
        {
            // Arrange
            using (var context = new ApplicationDbContext(_options))
            {
                context.Set<Project>().Add(new Project { Name = "UniqueProject", Code = "UP" });
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _repository.GetByConditionAsync(p => p.Code == "UP");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("UniqueProject", result.Name);
        }

        [TestMethod]
        public async Task GetByIdAsync_WithIncludes_ShouldLoadRelatedData()
        {
            // Arrange
            int projectId;
            using (var context = new ApplicationDbContext(_options))
            {
                var project = new Project { Name = "Project with Tasks", Code = "PT" };
                project.Tasks = new List<PmsTask>
                {
                    new PmsTask { Summary = "Task 1", Description = "Desc" }
                };

                context.Set<Project>().Add(project);
                await context.SaveChangesAsync();
                projectId = project.Id;
            }

            // Act - завантажуємо проект разом із завданнями
            var result = await _repository.GetByIdAsync(projectId, p => p.Tasks);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Tasks);
            Assert.AreEqual(1, result.Tasks.Count);
            Assert.AreEqual("Task 1", result.Tasks.First().Summary);
        }

        [TestMethod]
        public async Task GetAllAsync_WithIncludesAndPredicate_ShouldReturnFullData()
        {
            // Arrange
            using (var context = new ApplicationDbContext(_options))
            {
                var project = new Project { Name = "Active Project", Code = "AP" };
                project.Sprints = new List<Sprint>
                {
                    new Sprint { Name = "Sprint 1" }
                };
                context.Set<Project>().Add(project);
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _repository.GetAllAsync(
                p => p.Code == "AP",
                p => p.Sprints
            );

            // Assert
            var projectResult = result.FirstOrDefault();
            Assert.IsNotNull(projectResult);
            Assert.IsNotNull(projectResult.Sprints);
            Assert.AreEqual("Sprint 1", projectResult.Sprints.First().Name);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task GetByIdAsync_NonExistingId_ShouldThrowException()
        {
            // Act
            await _repository.GetByIdAsync(999);
            // Assert - ExpectedException handles this
        }
    }
}
