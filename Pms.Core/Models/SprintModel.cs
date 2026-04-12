using Pms.Core.Entities;

namespace Pms.Core.Models
{
    public class SprintModel
    {
        public SprintModel()
        {
        }

        public SprintModel(Sprint sprint) : this()
        {
            this.Id = sprint.Id;
            this.Name = sprint.Name;
            this.Status = sprint.Status;
            this.ProjId = sprint.Project.Id;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }
        public int ProjId { get; set; }
    }
}
