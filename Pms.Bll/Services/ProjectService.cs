using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> userManager;

        public ProjectService(IRepository<Project> projectRepos, IRepository<Participant> participntRepos)
        {
            this.projectRepos = projectRepos;
            this.participntRepos = participntRepos;
        }

        public async Task CreateProject(ProjectModel model)
        {
            var existing = await this.projectRepos.GetByConditionAsync(p => p.Name == model.Name);
            if (existing != null)
            {
                throw new InvalidOperationException($"Project with name '{model.Name}' already exists.");
            }

            var project = new Project(model);
            await this.projectRepos.CreateAsync(project);
            project.Code = $"Proj-{project.Id}";
        }

        public async Task UpdateProject(ProjectModel model)
        {
            var duplicate = await this.projectRepos.GetByConditionAsync(p => p.Name == model.Name && p.Id != model.Id);

            if (duplicate != null)
            {
                throw new InvalidOperationException($"Project with name '{model.Name}' already exists.");
            }
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

        public async Task<IList<ProjectModel>> GetProjects(User user)
        {
            var userCanDeleteProjects = user.CanDeleteProject();
            var entities = user.CanViewAllProjects()
                ? await this.projectRepos.GetAllAsync()
                : await this.projectRepos.GetAllAsync(p => p.Participants.Any(pp => pp.UserId == user.Id));
            return entities.Select(e => new ProjectModel(e) { CanDelete = userCanDeleteProjects }).ToList();
        }

        public Task DeleteParticipant(int id)
        {
            return this.participntRepos.DeleteAsync(id);
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

        public async Task<ProjectModel> GetProject(int id)
        {
            var project = await this.projectRepos.GetByIdAsync(id);
            return new ProjectModel(project);
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
