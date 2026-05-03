using Pms.Bll.Interfaces;
using Pms.Core.Entities;
using Pms.Core.Interfaces;
using Pms.Core.Models;

namespace Pms.Bll.Services
{
    public class SprintService : ISprintService
    {
        private readonly IRepository<Sprint> sprintRepos;

        public SprintService(IRepository<Sprint> sprintRepos)
        {
            this.sprintRepos = sprintRepos;
        }

        public async Task CreateSprint(SprintModel model)
        {
            var nameExists = await this.sprintRepos.GetByConditionAsync(s => s.ProjectId == model.ProjId && s.Name == model.Name);

            if (nameExists != null)
            {
                throw new InvalidOperationException($"Sprint with name '{model.Name}' already exists in this project.");
            }

            var sprint = new Sprint(model);
            await this.sprintRepos.CreateAsync(sprint);
        }

        public async Task UpdateSprint(SprintModel model)
        {
            var duplicate = await this.sprintRepos.GetByConditionAsync(s => s.ProjectId == model.ProjId && s.Name == model.Name && s.Id != model.Id);

            if (duplicate != null)
            {
                throw new InvalidOperationException($"Sprint with name '{model.Name}' already exists in this project.");
            }

            var sprint = await this.sprintRepos.GetByIdAsync(model.Id);
            sprint.Name = model.Name;
            sprint.Status = model.Status;
            await this.sprintRepos.UpdateAsync(sprint);
        }

        public Task DeleteSprint(int id)
        {
            return this.sprintRepos.DeleteAsync(id);
        }

        public async Task<IList<SprintModel>> GetSprints(int projectId)
        {
            var entities = await this.sprintRepos.GetAllAsync(t => t.ProjectId == projectId);
            return entities.Select(e => new SprintModel(e)).ToList();
        }

        public async Task<SprintModel> GetSprint(int id)
        {
            var sprint = await this.sprintRepos.GetByIdAsync(id);
            return new SprintModel(sprint);
        }
    }
}
