using Moq;
using Pms.Bll.Services;
using Pms.Core.Entities;
using Pms.Core.Enums;
using Pms.Core.Interfaces;
using Pms.Core.Models;

namespace Pms.Tests
{
    [TestClass]
    public class TaskServiceTests
    {
        private Mock<IRepository<PmsTask>> _taskRepoMock;
        private TaskService _service;

        [TestInitialize]
        public void Setup()
        {
            _taskRepoMock = new Mock<IRepository<PmsTask>>();
            _service = new TaskService(_taskRepoMock.Object);
        }

        [TestMethod]
        public async Task CreateTask_ShouldInvokeRepository_WithCorrectData()
        {
            // Arrange
            var model = new TaskModel
            {
                Summary = "Fix bug",
                TaskType = TaskType.Bug,
                Priority = Priority.High
            };

            // Act
            await _service.CreateTask(model);

            // Assert
            _taskRepoMock.Verify(r => r.CreateAsync(It.Is<PmsTask>(t =>
                t.Summary == model.Summary &&
                t.TaskType == model.TaskType &&
                t.Priority == model.Priority)), Times.Once);
        }

        [TestMethod]
        public async Task UpdateTask_WhenTaskExists_ShouldUpdateAllFields()
        {
            // Arrange
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
                SprintId = 5
            };

            // Act
            await _service.UpdateTask(model);

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
            var taskId = 42;

            // Act
            await _service.DeleteTask(taskId);

            // Assert
            _taskRepoMock.Verify(r => r.DeleteAsync(taskId), Times.Once);
        }

        [TestMethod]
        public async Task UpdateTask_ShouldAssignSprintIdCorrecty()
        {
            // Arrange
            var taskId = 1;
            var task = new PmsTask { Id = taskId };
            _taskRepoMock.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(task);

            var model = new TaskModel { Id = taskId, SprintId = 99 };

            // Act
            await _service.UpdateTask(model);

            // Assert
            Assert.AreEqual(99, task.SprintId);
            _taskRepoMock.Verify(r => r.UpdateAsync(It.Is<PmsTask>(t => t.SprintId == 99)), Times.Once);
        }
    }
}
