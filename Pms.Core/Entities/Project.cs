using Pms.Core.Models;

namespace Pms.Core.Entities
{
    public class Project : BaseEntity
    {
        public Project()
        {
            this.Participants = new List<Participant>();
            this.Sprints = new List<Sprint>();
            this.Tasks = new List<PmsTask>();
        }

        public Project(ProjectModel model) : this()
        {
            this.Name = model.Name;
            this.Code = model.Code;
            this.Status = model.Status;
        }

        public string Name { get; set; }

        public string Code { get; set; }

        public Status Status { get; set; }

        public virtual IList<Participant> Participants { get; set; }

        public virtual IList<Sprint> Sprints { get; set; }

        public virtual IList<PmsTask> Tasks { get; set; }
    }
}
