namespace Pms.Core.Models
{
    public class ParticipantModel
    {
        public int Id { get; set; }
        public int ProjId { get; set; }
        public string UserId { get; set; }
        public bool Manager { get; set; }
    }
}
