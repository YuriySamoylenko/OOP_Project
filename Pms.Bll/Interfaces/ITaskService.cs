using Pms.Core.Models;

namespace Pms.Bll.Interfaces
{
    public interface ITaskService
    {
        Task CreateTask(TaskModel model);

        Task UpdateTask(TaskModel model);

        Task DeleteTask(int id);

        Task<List<TaskModel>> GetTasks(int projectId);

        Task<TaskModel> GetTask(int id);
    }
}
