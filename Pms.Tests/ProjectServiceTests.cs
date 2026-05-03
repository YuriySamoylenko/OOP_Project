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
        private Mock<IRepository<Participant>> _participantRepoMock;
        private ProjectService _service;

        [TestInitialize]
        public void Setup()
        {
            _projectRepoMock = new Mock<IRepository<Project>>();
            _participantRepoMock = new Mock<IRepository<Participant>>();

            _service = new ProjectService(_projectRepoMock.Object, _participantRepoMock.Object);
        }

        #region Project Tests

        [TestMethod]
        public async Task CreateProject_ShouldCallRepoWithMappedEntity()
        {
            // Arrange
            var model = new ProjectModel { Name = "New Project", Status = Status.InProgress };

            // Act
            await _service.CreateProject(model);

            // Assert
            _projectRepoMock.Verify(r => r.CreateAsync(It.Is<Project>(p =>
                p.Name == model.Name && p.Status == model.Status)), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task CreateProject_DuplicateName_ShouldThrowException()
        {
            // Arrange
            var model = new ProjectModel { Name = "Duplicate" };
            _projectRepoMock.Setup(r => r.GetByConditionAsync(It.IsAny<Expression<Func<Project, bool>>>()))
                            .ReturnsAsync(new Project { Name = "Duplicate" });

            // Act
            await _service.CreateProject(model);

            // Assert - ExpectedException handles this
        }

        [TestMethod]
        public async Task UpdateProject_WhenExists_ShouldUpdateFields()
        {
            // Arrange
            var projectId = 1;
            var existingProject = new Project { Id = projectId, Name = "Old Name" };
            _projectRepoMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(existingProject);

            var model = new ProjectModel { Id = projectId, Name = "Updated Name", Status = Status.Completed };

            // Act
            await _service.UpdateProject(model);

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
            var projectId = 1;
            var model = new ProjectModel { Id = projectId, Name = "Existing Name" };

            // Імітуємо, що інший проект (Id = 2) вже має таке ім'я
            _projectRepoMock.Setup(r => r.GetByConditionAsync(It.IsAny<Expression<Func<Project, bool>>>()))
                            .ReturnsAsync(new Project { Id = 2, Name = "Existing Name" });

            // Act
            await _service.UpdateProject(model);
        }

        [TestMethod]
        public async Task DeleteProject_ShouldInvokeDelete()
        {
            // Act
            await _service.DeleteProject(5);

            // Assert
            _projectRepoMock.Verify(r => r.DeleteAsync(5), Times.Once);
        }

        #endregion

        #region Participant Tests

        [TestMethod]
        public async Task AddParticipant_ShouldCreateCorrectType_Member()
        {
            // Arrange
            var model = new ParticipantModel { ProjId = 1, UserId = "1", Manager = false };

            // Act
            await _service.AddParticipant(model);

            // Assert
            _participantRepoMock.Verify(r => r.CreateAsync(It.Is<Participant>(p =>
                p is Member && p.ProjectId == model.ProjId)), Times.Once);
        }

        [TestMethod]
        public async Task AddParticipant_ShouldCreateCorrectType_Manager()
        {
            // Arrange
            var model = new ParticipantModel { ProjId = 1, UserId = "1", Manager = true };

            // Act
            await _service.AddParticipant(model);

            // Assert
            _participantRepoMock.Verify(r => r.CreateAsync(It.Is<Participant>(p =>
                p is Manager && p.ProjectId == model.ProjId)), Times.Once);
        }

        [TestMethod]
        public async Task UpdateParticipant_WhenRoleChangesFromMemberToManager_ShouldDeleteAndCreate()
        {
            // Arrange
            var id = 10;
            var existingMember = new Member { Id = id, UserId = "5", ProjectId = 1 };
            _participantRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingMember);

            var model = new ParticipantModel { Id = id, UserId = "5", ProjId = 1, Manager = true };

            // Act
            await _service.UpdateParticipant(model);

            // Assert
            _participantRepoMock.Verify(r => r.DeleteAsync(id), Times.Once, "Old member should be deleted");
            _participantRepoMock.Verify(r => r.CreateAsync(It.Is<Participant>(p => p is Manager)), Times.Once, "New manager should be created");
        }

        [TestMethod]
        public async Task UpdateParticipant_WhenRoleDoesNotChange_ShouldDoNothing()
        {
            // Arrange
            var id = 10;
            var existingManager = new Manager { Id = id, UserId = "5", ProjectId = 1 };
            _participantRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingManager);

            var model = new ParticipantModel { Id = id, UserId = "5", ProjId = 1, Manager = true };

            // Act
            await _service.UpdateParticipant(model);

            // Assert
            _participantRepoMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
            _participantRepoMock.Verify(r => r.CreateAsync(It.IsAny<Participant>()), Times.Never);
        }

        [TestMethod]
        public async Task DeleteParticipant_ShouldInvokeDelete()
        {
            // Act
            await _service.DeleteParticipant(7);

            // Assert
            _participantRepoMock.Verify(r => r.DeleteAsync(7), Times.Once);
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
        public async Task GetParticipants_ShouldReturnMappedModelsWithUserDetails()
        {
            // Arrange
            var projId = 1;
            var participants = new List<Participant>
            {
                new Member {
                    Id = 1,
                    ProjectId = projId,
                    User = new User { LastName = "Bond", Email = "007@mi6.com" }
                }
            };

            _participantRepoMock.Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Participant, bool>>>(),
                    It.IsAny<Expression<Func<Participant, object>>[]>()
                ))
            .ReturnsAsync(participants);

            // Act
            var result = await _service.GetParticipants(projId);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Bond", result[0].UserName);
            Assert.AreEqual("007@mi6.com", result[0].UserEmail);
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

        [TestMethod]
        public async Task GetParticipant_ShouldCallRepoWithCorrectCondition()
        {
            // Arrange
            var projId = 1;
            var userId = "user-123";
            var participant = new Member { ProjectId = projId, UserId = userId };

            _participantRepoMock.Setup(r => r.GetByConditionAsync(
                It.IsAny<Expression<Func<Participant, bool>>>()))
                .ReturnsAsync(participant);

            // Act
            var result = await _service.GetParticipant(projId, userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(projId, result.ProjectId);
            Assert.AreEqual(userId, result.UserId);
            _participantRepoMock.Verify(r => r.GetByConditionAsync(It.IsAny<Expression<Func<Participant, bool>>>()), Times.Once);
        }

        #endregion
    }
}
