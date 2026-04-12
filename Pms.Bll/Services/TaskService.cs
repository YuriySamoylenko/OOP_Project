using Pms.Bll.Interfaces;
using Pms.Core.Entities;
using Pms.Core.Interfaces;
using Pms.Core.Models;

namespace Pms.Bll.Services
{
    public class TaskService : ITaskService
    {
        private readonly IRepository<PmsTask> taskRepos;

        public TaskService(IRepository<PmsTask> taskRepos)
        {
            this.taskRepos = taskRepos;
        }

        public Task CreateTask(TaskModel model)
        {
            var task = new PmsTask(model);
            return this.taskRepos.CreateAsync(task);
        }

        public async Task UpdateTask(TaskModel model)
        {
            var task = await this.taskRepos.GetByIdAsync(model.Id);
            task.Summary = model.Summary;
            task.TaskType = model.TaskType;
            task.Status = model.Status;
            task.Priority = model.Priority;
            task.Severity = model.Severity;
            task.SprintId = model.SprintId;
            await this.taskRepos.UpdateAsync(task);
        }

        public Task DeleteTask(int id)
        {
            return this.taskRepos.DeleteAsync(id);
        }
    }
}
