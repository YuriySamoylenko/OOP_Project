using Moq;
using Pms.Bll.Interfaces;
using Pms.Bll.Services;
using Pms.Core.Entities;
using Pms.Core.Enums;
using Pms.Core.Interfaces;
using Pms.Core.Models;
using System.Linq.Expressions;

namespace Pms.Tests
{
    [TestClass]
    public class SprintServiceTests
    {
        private Mock<IRepository<Sprint>> _sprintRepoMock;
        private Mock<IParticipantService> _participantServiceMock;
        private SprintService _service;

        [TestInitialize]
        public void Setup()
        {
            _sprintRepoMock = new Mock<IRepository<Sprint>>();

            _participantServiceMock = new Mock<IParticipantService>();

            _service = new SprintService(_sprintRepoMock.Object, _participantServiceMock.Object);
        }

        [TestMethod]
        public async Task CreateSprint_ShouldCallRepository_WithMappedEntity()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };
            var model = new SprintModel
            {
                Name = "Sprint 1",
                Status = Status.New,
                ProjId = 1
            };
            _participantServiceMock.Setup(m => m.ParticipantCanManageSprints(user.Id, model.ProjId)).ReturnsAsync(true);

            // Act
            await _service.CreateSprint(model, user);

            // Assert
            _sprintRepoMock.Verify(r => r.CreateAsync(It.Is<Sprint>(s => s.Name == model.Name)), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task CreateSprint_DuplicateNameInSameProject_ShouldThrowException()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };
            var model = new SprintModel { Name = "Sprint 1", ProjId = 1 };

            _participantServiceMock.Setup(m => m.ParticipantCanManageSprints(user.Id, model.ProjId)).ReturnsAsync(true);
            _sprintRepoMock.Setup(r => r.GetByConditionAsync(It.IsAny<Expression<Func<Sprint, bool>>>()))
                            .ReturnsAsync(new Sprint { Name = "Sprint 1", ProjectId = 1 });

            // Act
            await _service.CreateSprint(model, user);
        }

        [TestMethod]
        public async Task UpdateSprint_WhenSprintExists_ShouldUpdateFieldsAndSave()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };
            var sprintId = 10;
            var existingSprint = new Sprint { Id = sprintId, Name = "Old Name" };

            var model = new SprintModel
            {
                Id = sprintId,
                Name = "New Name",
                Status = Status.InProgress,
            };

            _participantServiceMock.Setup(m => m.ParticipantCanManageSprints(user.Id, model.ProjId)).ReturnsAsync(true);
            _sprintRepoMock.Setup(r => r.GetByIdAsync(sprintId))
                           .ReturnsAsync(existingSprint);

            // Act
            await _service.UpdateSprint(model, user);

            // Assert
            Assert.AreEqual("New Name", existingSprint.Name);
            Assert.AreEqual(Status.InProgress, existingSprint.Status);

            _sprintRepoMock.Verify(r => r.UpdateAsync(existingSprint), Times.Once);
        }

        [TestMethod]
        public async Task DeleteSprint_ShouldCallRepositoryDelete()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };
            var sprintId = 5;
            _participantServiceMock.Setup(m => m.ParticipantCanManageSprints(user.Id, 1)).ReturnsAsync(true);

            // Act
            await _service.DeleteSprint(sprintId, 1, user);

            // Assert
            _sprintRepoMock.Verify(r => r.DeleteAsync(sprintId), Times.Once);
        }

        [TestMethod]
        public async Task UpdateSprint_WhenSprintNotFound_ShouldNotCallUpdate()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };
            var sprintId = 1;

            var model = new SprintModel { Id = sprintId, ProjId = 1 };
            _participantServiceMock.Setup(m => m.ParticipantCanManageSprints(user.Id, model.ProjId)).ReturnsAsync(true);
            _sprintRepoMock.Setup(r => r.GetByIdAsync(sprintId))
                           .ReturnsAsync((Sprint)null!);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<System.NullReferenceException>(async () =>
                await _service.UpdateSprint(model, user));

            _sprintRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Sprint>()), Times.Never);
        }

        [TestMethod]
        public async Task GetSprints_ShouldReturnFilteredList_MappedToModel()
        {
            // Arrange
            var projectId = 1;
            var sprints = new List<Sprint>
            {
                new Sprint { Id = 1, Name = "Sprint 1", ProjectId = projectId },
                new Sprint { Id = 2, Name = "Sprint 2", ProjectId = projectId }
            };

            _sprintRepoMock.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<Sprint, bool>>>(),
                It.IsAny<Expression<Func<Sprint, object>>[]>()))
                .ReturnsAsync(sprints);

            // Act
            var result = await _service.GetSprints(projectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Sprint 1", result[0].Name);
            Assert.AreEqual("Sprint 2", result[1].Name);

            _sprintRepoMock.Verify(r => r.GetAllAsync(
                It.IsAny<Expression<Func<Sprint, bool>>>(),
                It.IsAny<Expression<Func<Sprint, object>>[]>()), Times.Once);
        }

        [TestMethod]
        public async Task GetSprint_WhenExists_ShouldReturnMappedModel()
        {
            // Arrange
            var sprintId = 1;
            var sprint = new Sprint { Id = sprintId, Name = "Existing Sprint" };

            _sprintRepoMock.Setup(r => r.GetByIdAsync(sprintId))
                           .ReturnsAsync(sprint);

            // Act
            var result = await _service.GetSprint(sprintId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(sprintId, result.Id);
            Assert.AreEqual("Existing Sprint", result.Name);
            _sprintRepoMock.Verify(r => r.GetByIdAsync(sprintId), Times.Once);
        }

        [TestMethod]
        public async Task GetSprint_WhenNotFound_ShouldReturnModelWithNullBase()
        {
            // Arrange
            var sprintId = 99;
            _sprintRepoMock.Setup(r => r.GetByIdAsync(sprintId))
                           .ReturnsAsync((Sprint)null!);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<NullReferenceException>(async () =>
                await _service.GetSprint(sprintId));
        }
    }
}
