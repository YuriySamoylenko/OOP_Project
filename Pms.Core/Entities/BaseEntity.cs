namespace Pms.Core.Entities
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            this.Created = DateTime.UtcNow;
            this.Updated = this.Created;
        }

        public int Id { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
    }
}
