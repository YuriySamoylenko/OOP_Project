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

        public virtual bool CanManageProject() => false;

        public virtual bool CanManageUsers() => false;

        public virtual bool CanManageSprints() => false;

        public virtual bool CanManageTasks() => false;
    }
}
