namespace Pms.Core.Entities
{
    public class Manager : Participant
    {
        public virtual bool CanManageParticipants()
        {
            return true;
        }

        public virtual bool CanManageSprints()
        {
            return true;
        }
    }
}
