namespace Pms.Core.Entities
{
    public class Manager : Participant
    {
        public override bool CanManageParticipants()
        {
            return true;
        }

        public override bool CanManageSprints()
        {
            return true;
        }
    }
}
