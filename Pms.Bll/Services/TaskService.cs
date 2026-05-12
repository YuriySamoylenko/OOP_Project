using Pms.Bll.Interfaces;
using Pms.Core.Entities;
using Pms.Core.Interfaces;
using Pms.Core.Models;

namespace Pms.Bll.Services
{
    public class TaskService : ITaskService
    {
        private readonly IRepository<PmsTask> taskRepos;

        private readonly IParticipantService participantService;

        public TaskService(IRepository<PmsTask> taskRepos, IParticipantService participantService)
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

        private void ThrowAccessDenied()
        {
            throw new UnauthorizedAccessException("User can not perform this action.");
        }
    }
}
