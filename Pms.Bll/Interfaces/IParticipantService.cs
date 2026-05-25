using Pms.Core.Entities;
using Pms.Core.Models;

namespace Pms.Bll.Interfaces
{
    public interface IParticipantService
    {
        Task AddParticipant(ParticipantModel model, User user);

        Task UpdateParticipant(ParticipantModel model, User user);

        Task DeleteParticipant(int id, int projId, User user);

        Task<IList<ParticipantModel>> GetParticipants(int projId);

        Task<Participant> GetParticipant(int projId, string userId);
    }
}
