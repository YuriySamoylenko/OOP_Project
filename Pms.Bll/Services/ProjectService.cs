using Pms.Bll.Interfaces;
using Pms.Core.Entities;
using Pms.Core.Interfaces;
using Pms.Core.Models;

namespace Pms.Bll.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IRepository<Project> projectRepos;

        public ProjectService(IRepository<Project> projectRepos)
        {
            this.projectRepos = projectRepos;
        }

        public async Task CreateProject(ProjectModel model, User user)
        {
            if (!user.CanManageProject())
            {
                this.ThrowAccessDenied();
            }

            var existing = await this.projectRepos.GetByConditionAsync(p => p.Name == model.Name);
            if (existing != null)
            {
                throw new InvalidOperationException($"Project with name '{model.Name}' already exists.");
            }

            var project = new Project(model);
            project.Code = "";
            await this.projectRepos.CreateAsync(project);
            project.Code = $"Proj-{project.Id}";
        }

        public async Task UpdateProject(ProjectModel model, User user)
        {
            if (!user.CanManageProject())
            {
                this.ThrowAccessDenied();
            }

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

        public Task DeleteProject(int id, User user)
        {
            if (!user.CanManageProject())
            {
                this.ThrowAccessDenied();
            }

            return this.projectRepos.DeleteAsync(id);
        }

        public async Task<IList<ProjectModel>> GetProjects(User user)
        {
            var entities = user.CanManageProject()
                ? await this.projectRepos.GetAllAsync()
                : await this.projectRepos.GetAllAsync(p => p.Participants.Any(pp => pp.UserId == user.Id));
            return entities.Select(e => new ProjectModel(e)).ToList();
        }

        public async Task<ProjectModel> GetProject(int id)
        {
            var project = await this.projectRepos.GetByIdAsync(id);
            return new ProjectModel(project);
        }

        private void ThrowAccessDenied()
        {
            throw new UnauthorizedAccessException("User can not perform this action.");
        }
    }
}
