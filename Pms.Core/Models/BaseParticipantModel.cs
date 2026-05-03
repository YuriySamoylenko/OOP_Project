using Pms.Core.Entities;

namespace Pms.Core.Models
{
    public class BaseParticipantModel
    {
        public BaseParticipantModel()
        {
        }

        public BaseParticipantModel(Participant participant)
        {
            this.Id = participant.Id;
            this.ProjId = participant.ProjectId;
            this.UserId = participant.UserId;
            this.Manager = participant is Manager;
        }

        public int Id { get; set; }
        public int ProjId { get; set; }
        public string UserId { get; set; }
        public bool Manager { get; set; }
    }
}
