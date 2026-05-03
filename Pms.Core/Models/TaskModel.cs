using Pms.Core.Entities;
using Pms.Core.Enums;
using System.ComponentModel.DataAnnotations;

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
            this.SprintId = task.SprintId;
            this.ProjId = task.ProjectId;
            this.AssigneeId = task.AssigneeId;
            this.CreatorId = task.CreatorId;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Summary is required")]
        [StringLength(256, MinimumLength = 5, ErrorMessage = "Summary must be between 5 and 256 characters")]
        public string Summary { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(4000, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 4000 characters")]
        public string Description { get; set; }
        public TaskType TaskType { get; set; }
        public PmsTaskStatus Status { get; set; }
        public Priority Priority { get; set; }
        public Severity Severity { get; set; }
        public int ProjId { get; set; }
        public int? SprintId { get; set; }

        public string? AssigneeId { get; set; }
        public string? CreatorId { get; set; }

        public IDictionary<string, string> Export()
        {
            return new Dictionary<string, string>
            {
                { nameof(Id), this.Id.ToString() },
                { nameof(Summary), this.Summary },
                { nameof(Description), this.Description },
                { nameof(TaskType), this.TaskType.ToString() },
                { nameof(Status), this.Status.ToString() },
                { nameof(Priority), this.Priority.ToString() },
                { nameof(Severity), this.Severity.ToString() },
            };
        }
    }
}
