using Pms.Bll.Interfaces;
using Pms.Core.Entities;
using Pms.Core.Interfaces;
using Pms.Core.Models;
using System.Text.Json;

namespace Pms.Bll.Services
{
    public class TaskService : ITaskService
    {
        private readonly IRepository<PmsTask> taskRepos;

        private readonly IProjectRoleService participantService;

        public TaskService(IRepository<PmsTask> taskRepos, IProjectRoleService participantService)
        {
            this.taskRepos = taskRepos;
            this.participantService = participantService;
        }

        public async Task CreateTask(TaskModel model, User user)
        {
            if (!user.CanManageSprints() && !await this.participantService.ParticipantCanManageTasks(user.Id, model.ProjId))
            {
                this.ThrowAccessDenied();
            }

            var task = new PmsTask(model);
            await this.taskRepos.CreateAsync(task);
        }

        public async Task UpdateTask(TaskModel model, User user)
        {
            if (!user.CanManageSprints() && !await this.participantService.ParticipantCanManageTasks(user.Id, model.ProjId))
            {
                this.ThrowAccessDenied();
            }

            var task = await this.taskRepos.GetByIdAsync(model.Id);
            task.Summary = model.Summary;
            task.TaskType = model.TaskType;
            task.Status = model.Status;
            task.Priority = model.Priority;
            task.Severity = model.Severity;
            task.SprintId = model.SprintId;
            task.AssigneeId = model.AssigneeId;
            await this.taskRepos.UpdateAsync(task);
        }

        public async Task DeleteTask(int id, int projId, User user)
        {
            if (!user.CanManageSprints() && !await this.participantService.ParticipantCanManageTasks(user.Id, projId))
            {
                this.ThrowAccessDenied();
            }

            await this.taskRepos.DeleteAsync(id);
        }

        public async Task<List<TaskModel>> GetTasks(int projectId)
        {
            var entities = await this.taskRepos.GetAllAsync(t => t.ProjectId == projectId);
            return entities.Select(e => new TaskModel(e)).ToList();
        }

        public async Task<TaskModel> GetTask(int id)
        {
            var task = await this.taskRepos.GetByIdAsync(id);
            return new TaskModel(task);
        }

        public async Task<string> ExportTaskToJsonAsync(int taskId)
        {
            var task = await this.taskRepos.GetByIdAsync(taskId);
            if (task == null)
            {
                throw new KeyNotFoundException("Task not found.");
            }

            var model = new TaskModel
            {
                Summary = task.Summary,
                Description = task.Description,
                TaskType = task.TaskType,
                Status = task.Status,
                Priority = task.Priority,
                Severity = task.Severity
            };

            var options = new JsonSerializerOptions { WriteIndented = true };

            return JsonSerializer.Serialize(model, options);
        }

        public async Task ImportTaskFromJsonAsync(string jsonContent, int targetProjectId, string creatorId)
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                throw new ArgumentException("File is empty.");
            }

            var model = JsonSerializer.Deserialize<TaskModel>(jsonContent);
            if (model == null)
            {
                throw new InvalidOperationException("Json format is not correct.");
            }

            model.ProjId = targetProjectId;
            model.CreatorId = creatorId;
            model.SprintId = null;

            var taskEntity = new PmsTask(model);

            await this.taskRepos.CreateAsync(taskEntity);
        }

        private void ThrowAccessDenied()
        {
            throw new UnauthorizedAccessException("User can not perform this action.");
        }
    }
}
