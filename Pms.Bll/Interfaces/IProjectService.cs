using Pms.Core.Models;

namespace Pms.Bll.Interfaces
{
    public interface IProjectService
    {
        Task CreateProject(ProjectModel model);

        Task UpdateProject(ProjectModel model);

        Task DeleteProject(int id);
    }
}
