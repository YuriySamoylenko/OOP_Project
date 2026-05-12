using Pms.Core.Entities;
using Pms.Core.Models;

namespace Pms.Bll.Interfaces
{
    public interface ISprintService
    {
        Task CreateSprint(SprintModel model, User user);

        Task UpdateSprint(SprintModel model, User user);

        Task DeleteSprint(int id, int projId, User user);

        Task<IList<SprintModel>> GetSprints(int projectId);

        Task<SprintModel> GetSprint(int id);
    }
}
