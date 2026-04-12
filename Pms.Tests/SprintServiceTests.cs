using Moq;
using Pms.Bll.Services;
using Pms.Core.Entities;
using Pms.Core.Interfaces;
using Pms.Core.Models;

namespace Pms.Tests
{
    [TestClass]
    public class SprintServiceTests
    {
        private Mock<IRepository<Sprint>> _sprintRepoMock;
        private SprintService _service;

        [TestInitialize]
        public void Setup()
        {
            _sprintRepoMock = new Mock<IRepository<Sprint>>();

            _service = new SprintService(_sprintRepoMock.Object);
        }

        [TestMethod]
        public async Task CreateSprint_ShouldCallRepository_WithMappedEntity()
        {
            // Arrange
            var model = new SprintModel
            {
                Name = "Sprint 1",
                Status = Status.New,
            };

            // Act
            await _service.CreateSprint(model);

            // Assert
            _sprintRepoMock.Verify(r => r.CreateAsync(It.Is<Sprint>(s => s.Name == model.Name)), Times.Once);
        }

        [TestMethod]
        public async Task UpdateSprint_WhenSprintExists_ShouldUpdateFieldsAndSave()
        {
            // Arrange
            var sprintId = 10;
            var existingSprint = new Sprint { Id = sprintId, Name = "Old Name" };

            _sprintRepoMock.Setup(r => r.GetByIdAsync(sprintId))
                           .ReturnsAsync(existingSprint);

            var model = new SprintModel
            {
                Id = sprintId,
                Name = "New Name",
                Status = Status.InProgress,
            };

            // Act
            await _service.UpdateSprint(model);

            // Assert
            Assert.AreEqual("New Name", existingSprint.Name);
            Assert.AreEqual(Status.InProgress, existingSprint.Status);

            _sprintRepoMock.Verify(r => r.UpdateAsync(existingSprint), Times.Once);
        }

        [TestMethod]
        public async Task DeleteSprint_ShouldCallRepositoryDelete()
        {
            // Arrange
            var sprintId = 5;

            // Act
            await _service.DeleteSprint(sprintId);

            // Assert
            _sprintRepoMock.Verify(r => r.DeleteAsync(sprintId), Times.Once);
        }

        [TestMethod]
        public async Task UpdateSprint_WhenSprintNotFound_ShouldNotCallUpdate()
        {
            // Arrange
            var sprintId = 1;
            _sprintRepoMock.Setup(r => r.GetByIdAsync(sprintId))
                           .ReturnsAsync((Sprint)null!);

            var model = new SprintModel { Id = sprintId };

            // Act & Assert
            await Assert.ThrowsExceptionAsync<System.NullReferenceException>(async () =>
                await _service.UpdateSprint(model));

            _sprintRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Sprint>()), Times.Never);
        }
    }
}
