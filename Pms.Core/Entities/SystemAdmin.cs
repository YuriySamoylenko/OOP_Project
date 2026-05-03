namespace Pms.Core.Entities
{
    public class SystemAdmin : User
    {
        public override bool CanViewAllProjects() => true;

        public override bool CanDeleteProject() => true;
    }
}
