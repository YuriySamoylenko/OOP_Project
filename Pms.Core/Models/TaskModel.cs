using Pms.Core.Entities;

namespace Pms.Core.Models
{
    public class TaskModel
    {
        public TaskModel()
        {
        }

        public TaskModel(PmsTask task) : this()
        {
            this.Id = task.Id;
            this.Summary = task.Summary;
            this.Description = task.Description;
            this.TaskType = task.TaskType;
            this.Status = task.Status;
            this.Priority = task.Priority;
            this.Severity = task.Severity;
            this.ProjId = task.ProjectId;
        }

        public int Id { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public TaskType TaskType { get; set; }
        public PmsTaskStatus Status { get; set; }
        public Priority Priority { get; set; }
        public Severity Severity { get; set; }
        public int ProjId { get; set; }
        public int SprintId { get; set; }
    }
}
