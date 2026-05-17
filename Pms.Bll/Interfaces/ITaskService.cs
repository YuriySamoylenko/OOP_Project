using Pms.Core.Entities;
using Pms.Core.Models;

namespace Pms.Bll.Interfaces
{
    public interface ITaskService
    {
        Task CreateTask(TaskModel model, User user);

        Task UpdateTask(TaskModel model, User user);

        Task DeleteTask(int id, int projId, User user);

        Task<List<TaskModel>> GetTasks(int projectId);

        Task<TaskModel> GetTask(int id);

        Task<string> ExportTaskToJsonAsync(int taskId);

        Task ImportTaskFromJsonAsync(string jsonContent, int targetProjectId, string creatorId);
    }
}
