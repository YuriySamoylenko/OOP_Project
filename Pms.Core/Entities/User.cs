using Microsoft.AspNetCore.Identity;

namespace Pms.Core.Entities
{
    public class User : IdentityUser
    {
        public User()
        {
            this.Participants = new List<Participant>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public virtual IList<Participant> Participants { get; set; }

        public virtual bool CanViewAllProjects() => false;

        public virtual bool CanDeleteProject() => false;
    }
}
