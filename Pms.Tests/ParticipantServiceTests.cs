using Moq;
using Pms.Bll.Services;
using Pms.Core.Entities;
using Pms.Core.Interfaces;
using Pms.Core.Models;
using System.Linq.Expressions;

namespace Pms.Tests
{
    [TestClass]
    public class ParticipantServiceTests
    {
        private Mock<IRepository<Participant>> _participantRepoMock;
        private ParticipantService _service;

        [TestInitialize]
        public void Setup()
        {
            _participantRepoMock = new Mock<IRepository<Participant>>();

            _service = new ParticipantService(_participantRepoMock.Object);
        }

        #region Participant Tests

        [TestMethod]
        public async Task AddParticipant_ShouldCreateCorrectType_Member()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };
            var model = new ParticipantModel { ProjId = 1, UserId = "1", Manager = false };

            // Act
            await _service.AddParticipant(model, user);

            // Assert
            _participantRepoMock.Verify(r => r.CreateAsync(It.Is<Participant>(p =>
                p is Member && p.ProjectId == model.ProjId)), Times.Once);
        }

        [TestMethod]
        public async Task AddParticipant_ShouldCreateCorrectType_Manager()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };
            var model = new ParticipantModel { ProjId = 1, UserId = "1", Manager = true };

            // Act
            await _service.AddParticipant(model, user);

            // Assert
            _participantRepoMock.Verify(r => r.CreateAsync(It.Is<Participant>(p =>
                p is Manager && p.ProjectId == model.ProjId)), Times.Once);
        }

        [TestMethod]
        public async Task UpdateParticipant_WhenRoleChangesFromMemberToManager_ShouldDeleteAndCreate()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };
            var id = 10;
            var existingMember = new Member { Id = id, UserId = "5", ProjectId = 1 };
            _participantRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingMember);

            var model = new ParticipantModel { Id = id, UserId = "5", ProjId = 1, Manager = true };

            // Act
            await _service.UpdateParticipant(model, user);

            // Assert
            _participantRepoMock.Verify(r => r.DeleteAsync(id), Times.Once, "Old member should be deleted");
            _participantRepoMock.Verify(r => r.CreateAsync(It.Is<Participant>(p => p is Manager)), Times.Once, "New manager should be created");
        }

        [TestMethod]
        public async Task UpdateParticipant_WhenRoleDoesNotChange_ShouldDoNothing()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };
            var id = 10;
            var existingManager = new Manager { Id = id, UserId = "5", ProjectId = 1 };
            _participantRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingManager);

            var model = new ParticipantModel { Id = id, UserId = "5", ProjId = 1, Manager = true };

            // Act
            await _service.UpdateParticipant(model, user);

            // Assert
            _participantRepoMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
            _participantRepoMock.Verify(r => r.CreateAsync(It.IsAny<Participant>()), Times.Never);
        }

        [TestMethod]
        public async Task DeleteParticipant_ShouldInvokeDelete()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };

            // Act
            await _service.DeleteParticipant(7, 1, user);

            // Assert
            _participantRepoMock.Verify(r => r.DeleteAsync(7), Times.Once);
        }

        #endregion

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
    }
}
