using Pms.Core.Entities;
using Pms.Core.Models;

namespace Pms.Bll.Interfaces
{
    public interface IProjectService
    {
        Task CreateProject(ProjectModel model, User user);

        Task UpdateProject(ProjectModel model, User user);

        Task DeleteProject(int id, User user);

        Task<IList<ProjectModel>> GetProjects(User user);

        Task<ProjectModel> GetProject(int id);
    }
}
