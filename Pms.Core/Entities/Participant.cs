namespace Pms.Core.Entities
{
    public class Participant : BaseEntity
    {
        public virtual int ProjectId { get; set; }

        public virtual Project Project { get; set; }

        public virtual string UserId { get; set; }

        public virtual User User { get; set; }

        public virtual bool CanManageParticipants()
        {
            return false;
        }

        public virtual bool CanManageSprints()
        {
            return false;
        }

        public virtual bool CanManageTasks()
        {
            return true;
        }
    }
}
