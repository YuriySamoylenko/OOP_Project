using Moq;
using Pms.Bll.Interfaces;
using Pms.Bll.Services;
using Pms.Core.Entities;
using Pms.Core.Enums;
using Pms.Core.Interfaces;
using Pms.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Pms.Tests
{
    [TestClass]
    public class TaskServiceTests
    {
        private Mock<IRepository<PmsTask>> _taskRepoMock;
        private Mock<IParticipantService> _participantServiceMock;
        private TaskService _service;

        [TestInitialize]
        public void Setup()
        {
            _taskRepoMock = new Mock<IRepository<PmsTask>>();

            _participantServiceMock = new Mock<IParticipantService>();
            _service = new TaskService(_taskRepoMock.Object, _participantServiceMock.Object);
        }

        [TestMethod]
        public async Task CreateTask_ShouldInvokeRepository_WithCorrectData()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };
            var model = new TaskModel
            {
                Summary = "Fix bug",
                TaskType = TaskType.Bug,
                Priority = Priority.High,
                ProjId = 1
            };
            _participantServiceMock.Setup(m => m.ParticipantCanManageTasks(user.Id, model.ProjId)).ReturnsAsync(true);

            // Act
            await _service.CreateTask(model, user);

            // Assert
            _taskRepoMock.Verify(r => r.CreateAsync(It.Is<PmsTask>(t =>
                t.Summary == model.Summary &&
                t.TaskType == model.TaskType &&
                t.Priority == model.Priority)), Times.Once);
        }

        [TestMethod]
        public void TaskModel_Validation_ShouldFail_WhenSummaryIsTooShort()
        {
            // Arrange
            var model = new TaskModel { Summary = "S", Description = "Valid Description" };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.IsFalse(isValid);
            Assert.IsTrue(results.Any(v => v.MemberNames.Contains("Summary")));
        }

        [TestMethod]
        public async Task UpdateTask_WhenTaskExists_ShouldUpdateAllFields()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };
            var taskId = 101;
            var existingTask = new PmsTask { Id = taskId, Summary = "Old Title" };

            _taskRepoMock.Setup(r => r.GetByIdAsync(taskId))
                         .ReturnsAsync(existingTask);

            var model = new TaskModel
            {
                Id = taskId,
                Summary = "New Title",
                TaskType = TaskType.Feature,
                Status = PmsTaskStatus.InProgress,
                Priority = Priority.Mid,
                Severity = Severity.Major,
                SprintId = 5,
                ProjId = 1
            };
            _participantServiceMock.Setup(m => m.ParticipantCanManageTasks(user.Id, model.ProjId)).ReturnsAsync(true);

            // Act
            await _service.UpdateTask(model, user);

            // Assert
            Assert.AreEqual(model.Summary, existingTask.Summary);
            Assert.AreEqual(model.Status, existingTask.Status);
            Assert.AreEqual(model.SprintId, existingTask.SprintId);

            _taskRepoMock.Verify(r => r.UpdateAsync(existingTask), Times.Once);
        }

        [TestMethod]
        public async Task DeleteTask_ShouldCallRepositoryDeleteAsync()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };
            var taskId = 42;
            _participantServiceMock.Setup(m => m.ParticipantCanManageTasks(user.Id, 1)).ReturnsAsync(true);

            // Act
            await _service.DeleteTask(taskId, 1, user);

            // Assert
            _taskRepoMock.Verify(r => r.DeleteAsync(taskId), Times.Once);
        }

        [TestMethod]
        public async Task UpdateTask_ShouldAssignSprintIdCorrecty()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };
            var taskId = 1;
            var task = new PmsTask { Id = taskId };
            _taskRepoMock.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(task);

            var model = new TaskModel { Id = taskId, SprintId = 99, ProjId = 1 };
            _participantServiceMock.Setup(m => m.ParticipantCanManageTasks(user.Id, model.ProjId)).ReturnsAsync(true);

            // Act
            await _service.UpdateTask(model, user);

            // Assert
            Assert.AreEqual(99, task.SprintId);
            _taskRepoMock.Verify(r => r.UpdateAsync(It.Is<PmsTask>(t => t.SprintId == 99)), Times.Once);
        }

        [TestMethod]
        public async Task GetTasks_ShouldReturnFilteredList_MappedToModels()
        {
            // Arrange
            var projectId = 10;
            var entities = new List<PmsTask>
            {
                new PmsTask { Id = 1, Summary = "Task 1", ProjectId = projectId },
                new PmsTask { Id = 2, Summary = "Task 2", ProjectId = projectId }
            };

            _taskRepoMock.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<PmsTask, bool>>>(),
                It.IsAny<Expression<Func<PmsTask, object>>[]>()))
                .ReturnsAsync(entities);

            // Act
            var result = await _service.GetTasks(projectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Task 1", result[0].Summary);
            _taskRepoMock.Verify(r => r.GetAllAsync(
                It.IsAny<Expression<Func<PmsTask, bool>>>(),
                It.IsAny<Expression<Func<PmsTask, object>>[]>()), Times.Once);
        }

        [TestMethod]
        public async Task GetTask_WhenExists_ShouldReturnMappedModel()
        {
            // Arrange
            var taskId = 1;
            var entity = new PmsTask { Id = taskId, Summary = "Single Task", AssigneeId = "user-123" };

            _taskRepoMock.Setup(r => r.GetByIdAsync(taskId))
                         .ReturnsAsync(entity);

            // Act
            var result = await _service.GetTask(taskId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(taskId, result.Id);
            Assert.AreEqual("Single Task", result.Summary);
            Assert.AreEqual("user-123", result.AssigneeId);
            _taskRepoMock.Verify(r => r.GetByIdAsync(taskId), Times.Once);
        }

        [TestMethod]
        public async Task UpdateTask_ShouldUpdateAssigneeId()
        {
            // Arrange
            var user = new SystemAdmin { Id = "u1" };
            var taskId = 5;
            var existingTask = new PmsTask { Id = taskId, AssigneeId = "old-user" };
            _taskRepoMock.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(existingTask);

            var model = new TaskModel { Id = taskId, AssigneeId = "new-user", ProjId = 1 };
            _participantServiceMock.Setup(m => m.ParticipantCanManageTasks(user.Id, model.ProjId)).ReturnsAsync(true);

            // Act
            await _service.UpdateTask(model, user);

            // Assert
            Assert.AreEqual("new-user", existingTask.AssigneeId);
            _taskRepoMock.Verify(r => r.UpdateAsync(It.Is<PmsTask>(t => t.AssigneeId == "new-user")), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task GetTask_WhenNotFound_ShouldThrowException()
        {
            // Arrange
            var taskId = 999;
            _taskRepoMock.Setup(r => r.GetByIdAsync(taskId))
                         .ReturnsAsync((PmsTask)null!);

            // Act
            await _service.GetTask(taskId);

            // Assert - ExpectedException
        }
    }
}
