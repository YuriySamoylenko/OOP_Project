using Pms.Core.Entities;
using Pms.Core.Enums;
using System.ComponentModel.DataAnnotations;

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
            this.ProjId = sprint.ProjectId;
        }

        public int Id { get; set; }
        [Required(ErrorMessage = "Sprint Name is Required")]
        [StringLength(16, MinimumLength = 3, ErrorMessage = "Sprint Name must be between 3 and 16 charackters")]
        public string Name { get; set; }
        public Status Status { get; set; }
        public int ProjId { get; set; }
    }
}
