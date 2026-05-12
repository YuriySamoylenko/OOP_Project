namespace Pms.Core.Entities
{
    public class SystemAdmin : User
    {
        public override bool CanManageProject() => true;

        public override bool CanManageUsers() => true;

        public override bool CanManageSprints() => true;

        public override bool CanManageTasks() => true;
    }
}
