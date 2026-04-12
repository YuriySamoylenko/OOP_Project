namespace Pms.Core.Entities
{
    public class Participant : BaseEntity
    {
        public virtual int ProjectId { get; set; }

        public virtual Project Project { get; set; }

        public virtual string UserId { get; set; }

        public virtual User User { get; set; }
    }
}
