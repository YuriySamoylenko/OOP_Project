using Pms.Core.Models;

namespace Pms.Core.Entities
{
    public class Sprint : BaseEntity
    {
        public Sprint()
        {
            this.Tasks = new List<PmsTask>();
        }

        public Sprint(SprintModel model) : this()
        {
            this.Name = model.Name;
            this.Status = model.Status;
            this.ProjectId = model.ProjId;
        }

        public string Name { get; set; }

        public Status Status { get; set; }

        public virtual int ProjectId { get; set; }

        public virtual Project Project { get; set; }

        public virtual IList<PmsTask> Tasks { get; set; }
    }
}
