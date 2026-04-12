using Pms.Bll.Interfaces;
using Pms.Core.Entities;
using Pms.Core.Interfaces;
using Pms.Core.Models;

namespace Pms.Bll.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IRepository<Project> projectRepos;
        private readonly IRepository<Participant> participntRepos;

        public ProjectService(IRepository<Project> projectRepos, IRepository<Participant> participntRepos)
        {
            this.projectRepos = projectRepos;
            this.participntRepos = participntRepos;
        }

        public Task CreateProject(ProjectModel model)
        {
            var project = new Project(model);
            return this.projectRepos.CreateAsync(project);
        }

        public async Task UpdateProject(ProjectModel model)
        {
            var project = await this.projectRepos.GetByIdAsync(model.Id);
            project.Name = model.Name;
            project.Status = model.Status;
            await this.projectRepos.UpdateAsync(project);
        }

        public Task DeleteProject(int id)
        {
            return this.projectRepos.DeleteAsync(id);
        }

        public Task AddParticipant(ParticipantModel model)
        {
            Participant participant = this.CreateParticipant(model);
            return this.participntRepos.CreateAsync(participant);
        }

        public async Task UpdateParticipant(ParticipantModel model)
        {
            var existingParticipant = await this.participntRepos.GetByIdAsync(model.Id);
            if (existingParticipant != null && ((model.Manager && existingParticipant is Member)
                || (!model.Manager && existingParticipant is Manager)))
            {
                await this.participntRepos.DeleteAsync(existingParticipant.Id);
                Participant participant = this.CreateParticipant(model);
                await this.participntRepos.CreateAsync(participant);
            }
        }

        public Task DeleteParticipant(int id)
        {
            return this.participntRepos.DeleteAsync(id);
        }

        private Participant CreateParticipant(ParticipantModel model)
        {

            Participant participant = model.Manager ? new Manager() : new Member();
            participant.ProjectId = model.ProjId;
            participant.UserId = model.UserId;
            return participant;
        }
    }
}
