using Pms.Core.Entities;
using Pms.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Pms.Core.Models
{
    public class ProjectModel
    {
        public ProjectModel()
        {
        }

        public ProjectModel(Project project) : this()
        {
            this.Id = project.Id;
            this.Name = project.Name;
            this.Code = project.Code;
            this.Status = project.Status;
            this.Created = project.Created;
        }

        public int Id { get; set; }
        [Required(ErrorMessage = "Project name is required")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
        public string Name { get; set; }
        public string? Code { get; set; }
        [Required(ErrorMessage = "Please select a status")]
        public Status Status { get; set; }
        public DateTime Created { get; }
    }
}
