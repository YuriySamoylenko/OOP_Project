using Pms.Core.Entities;
using Pms.Core.Models;

namespace Pms.Bll.Interfaces
{
    public interface IProjectService
    {
        Task CreateProject(ProjectModel model);

        Task UpdateProject(ProjectModel model);

        Task DeleteProject(int id);

        Task<IList<ProjectModel>> GetProjects(User user);

        Task<ProjectModel> GetProject(int id);

        Task AddParticipant(ParticipantModel model);

        Task UpdateParticipant(ParticipantModel model);

        Task DeleteParticipant(int id);

        Task<IList<ParticipantModel>> GetParticipants(int projId);

        Task<Participant> GetParticipant(int projId, string userId);
    }
}
