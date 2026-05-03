using Pms.Core.Models;

namespace Pms.Bll.Interfaces
{
    public interface ISprintService
    {
        Task CreateSprint(SprintModel model);

        Task UpdateSprint(SprintModel model);

        Task DeleteSprint(int id);

        Task<IList<SprintModel>> GetSprints(int projectId);

        Task<SprintModel> GetSprint(int id);
    }
}
