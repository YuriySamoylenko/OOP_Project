using Moq;
using Pms.Bll.Services;
using Pms.Core.Entities;
using Pms.Core.Enums;
using Pms.Core.Interfaces;
using Pms.Core.Models;
using System.Linq.Expressions;

namespace Pms.Tests
{
    [TestClass]
    public class ProjectServiceTests
    {
        private Mock<IRepository<Project>> _projectRepoMock;
        private ProjectService _service;

        [TestInitialize]
        public void Setup()
        {
            _projectRepoMock = new Mock<IRepository<Project>>();

            _service = new ProjectService(_projectRepoMock.Object);
        }

        #region Project Tests

        [TestMethod]
        public async Task CreateProject_ShouldCallRepoWithMappedEntity()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };
            var model = new ProjectModel { Name = "New Project", Status = Status.InProgress };

            // Act
            await _service.CreateProject(model, user);

            // Assert
            _projectRepoMock.Verify(r => r.CreateAsync(It.Is<Project>(p =>
                p.Name == model.Name && p.Status == model.Status)), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task CreateProject_DuplicateName_ShouldThrowException()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };
            var model = new ProjectModel { Name = "Duplicate" };
            _projectRepoMock.Setup(r => r.GetByConditionAsync(It.IsAny<Expression<Func<Project, bool>>>()))
                            .ReturnsAsync(new Project { Name = "Duplicate" });

            // Act
            await _service.CreateProject(model, user);

            // Assert - ExpectedException handles this
        }

        [TestMethod]
        public async Task UpdateProject_WhenExists_ShouldUpdateFields()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };
            var projectId = 1;
            var existingProject = new Project { Id = projectId, Name = "Old Name" };
            _projectRepoMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(existingProject);

            var model = new ProjectModel { Id = projectId, Name = "Updated Name", Status = Status.Completed };

            // Act
            await _service.UpdateProject(model, user);

            // Assert
            Assert.AreEqual("Updated Name", existingProject.Name);
            Assert.AreEqual(Status.Completed, existingProject.Status);
            _projectRepoMock.Verify(r => r.UpdateAsync(existingProject), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task UpdateProject_DuplicateName_ShouldThrowException()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };
            var projectId = 1;
            var model = new ProjectModel { Id = projectId, Name = "Existing Name" };

            _projectRepoMock.Setup(r => r.GetByConditionAsync(It.IsAny<Expression<Func<Project, bool>>>()))
                            .ReturnsAsync(new Project { Id = 2, Name = "Existing Name" });

            // Act
            await _service.UpdateProject(model, user);
        }

        [TestMethod]
        public async Task DeleteProject_ShouldInvokeDelete()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };

            // Act
            await _service.DeleteProject(5, user);

            // Assert
            _projectRepoMock.Verify(r => r.DeleteAsync(5), Times.Once);
        }

        #endregion

        #region Read Operations Tests

        [TestMethod]
        public async Task GetProjects_WhenUserCanViewAll_ShouldReturnAllProjects()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };

            var projects = new List<Project>
            {
                new Project { Id = 1, Name = "P1" },
                new Project { Id = 2, Name = "P2" }
            };

            _projectRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(projects);

            // Act
            var result = await _service.GetProjects(user);

            // Assert
            Assert.AreEqual(2, result.Count);
            _projectRepoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetProject_ShouldReturnCorrectModel()
        {
            // Arrange
            var projectId = 1;
            var project = new Project { Id = projectId, Name = "Test Project" };
            _projectRepoMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);

            // Act
            var result = await _service.GetProject(projectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Project", result.Name);
            _projectRepoMock.Verify(r => r.GetByIdAsync(projectId), Times.Once);
        }

        #endregion
    }
}
