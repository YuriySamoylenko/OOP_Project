using Pms.Core.Entities;
using Pms.Core.Enums;

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
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Status Status { get; set; }
    }
}
