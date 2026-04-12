using Pms.Core.Models;

namespace Pms.Core.Entities
{
    public class PmsTask : BaseEntity
    {
        public PmsTask()
        {
        }

        public PmsTask(TaskModel model) : this()
        {
            this.Summary = model.Summary;
            this.Description = model.Description;
            this.TaskType = model.TaskType;
            this.Status = model.Status;
            this.Priority = model.Priority;
            this.Severity = model.Severity;
        }

        public string Summary { get; set; }

        public string Description { get; set; }

        public TaskType TaskType { get; set; }

        public PmsTaskStatus Status { get; set; }

        public Priority Priority { get; set; }

        public Severity Severity { get; set; }

        public virtual string? AssigneeId { get; set; }

        public virtual User? Assignee { get; set; }

        public virtual string? CreatorId { get; set; }

        public virtual User? Creator { get; set; }

        public virtual int? SprintId { get; set; }

        public virtual Sprint? Sprint { get; set; }

        public virtual int ProjectId { get; set; }

        public virtual Project Project { get; set; }
    }
}
