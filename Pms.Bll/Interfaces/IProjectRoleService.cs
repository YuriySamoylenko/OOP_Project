namespace Pms.Bll.Interfaces
{
    public interface IProjectRoleService
    {
        Task<bool> ParticipantCanManageParticipants(string userId, int projId);

        Task<bool> ParticipantCanManageSprints(string userId, int projId);

        Task<bool> ParticipantCanManageTasks(string userId, int projId);
    }
}
