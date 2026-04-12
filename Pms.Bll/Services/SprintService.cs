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

        public Task CreateSprint(SprintModel model)
        {
            var sprint = new Sprint(model);
            return this.sprintRepos.CreateAsync(sprint);
        }

        public async Task UpdateSprint(SprintModel model)
        {
            var sprint = await this.sprintRepos.GetByIdAsync(model.Id);
            sprint.Name = model.Name;
            sprint.Status = model.Status;
            await this.sprintRepos.UpdateAsync(sprint);
        }

        public Task DeleteSprint(int id)
        {
            return this.sprintRepos.DeleteAsync(id);
        }
    }
}
