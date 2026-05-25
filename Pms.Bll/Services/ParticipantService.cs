using Pms.Bll.Interfaces;
using Pms.Core.Entities;
using Pms.Core.Interfaces;
using Pms.Core.Models;

namespace Pms.Bll.Services
{
    public class ParticipantService : IParticipantService, IProjectRoleService
    {
        private readonly IRepository<Participant> participntRepos;

        public ParticipantService(IRepository<Participant> participntRepos)
        {
            this.participntRepos = participntRepos;
        }

        public async Task AddParticipant(ParticipantModel model, User user)
        {
            if (!user.CanManageUsers() && !await this.ParticipantCanManageParticipants(user.Id, model.ProjId))
            {
                this.ThrowAccessDenied();
            }

            Participant participant = this.CreateParticipant(model);
            await this.participntRepos.CreateAsync(participant);
        }

        public async Task UpdateParticipant(ParticipantModel model, User user)
        {
            if (!user.CanManageUsers() && !await this.ParticipantCanManageParticipants(user.Id, model.ProjId))
            {
                this.ThrowAccessDenied();
            }

            var existingParticipant = await this.participntRepos.GetByIdAsync(model.Id);
            if (existingParticipant != null && ((model.Manager && existingParticipant is Member)
                || (!model.Manager && existingParticipant is Manager)))
            {
                await this.participntRepos.DeleteAsync(existingParticipant.Id);
                Participant participant = this.CreateParticipant(model);
                await this.participntRepos.CreateAsync(participant);
            }
        }

        public async Task DeleteParticipant(int id, int projId, User user)
        {
            if (!user.CanManageUsers() && !await this.ParticipantCanManageParticipants(user.Id, projId))
            {
                this.ThrowAccessDenied();
            }

            await this.participntRepos.DeleteAsync(id);
        }

        public async Task<IList<ParticipantModel>> GetParticipants(int projId)
        {
            var entities = await this.participntRepos.GetAllAsync(p => p.ProjectId == projId, p => p.User);
            return entities.Select(e => new ParticipantModel(e, e.User.LastName, e.User.Email)).ToList();
        }

        public async Task<Participant> GetParticipant(int projId, string userId)
        {
            return await this.participntRepos.GetByConditionAsync(p => p.ProjectId == projId && p.UserId == userId);
        }

        public async Task<bool> ParticipantCanManageParticipants(string userId, int projId)
        {
            var participant = await this.participntRepos.GetByConditionAsync(p => p.ProjectId == projId && p.UserId == userId);
            return participant != null && participant.CanManageParticipants();
        }

        public async Task<bool> ParticipantCanManageSprints(string userId, int projId)
        {
            var participant = await this.participntRepos
                .GetByConditionAsync(p => p.ProjectId == projId && p.UserId == userId);
            return participant != null && participant.CanManageSprints();
        }

        public async Task<bool> ParticipantCanManageTasks(string userId, int projId)
        {
            var participant = await this.participntRepos.GetByConditionAsync(p => p.ProjectId == projId && p.UserId == userId);
            return participant != null && participant.CanManageTasks();
        }

        private Participant CreateParticipant(ParticipantModel model)
        {
            Participant participant = model.Manager ? new Manager() : new Member();
            participant.ProjectId = model.ProjId;
            participant.UserId = model.UserId;
            return participant;
        }

        private void ThrowAccessDenied()
        {
            throw new UnauthorizedAccessException("User can not perform this action.");
        }
    }
}
