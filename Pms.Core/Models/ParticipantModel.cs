using Pms.Core.Entities;

namespace Pms.Core.Models
{
    public class ParticipantModel
    {
        public ParticipantModel()
        {
        }

        public ParticipantModel(Participant participant)
        {
            this.Id = participant.Id;
            this.ProjId = participant.ProjectId;
            this.UserId = participant.UserId;
            this.Manager = participant is Manager;
        }

        public ParticipantModel(Participant participant, string userName, string userEmail)
        {
            this.Id = participant.Id;
            this.ProjId = participant.ProjectId;
            this.UserId = participant.UserId;
            this.Manager = participant is Manager;
            this.UserName = userName;
            this.UserEmail = userEmail;
        }

        public int Id { get; set; }
        public int ProjId { get; set; }
        public string UserId { get; set; }
        public bool Manager { get; set; }

        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
    }
}
