using System;
namespace OTITO_Services.Model
{
    public partial class Activity
    {
        public int Id { get; set; }
        public string activity_type { get; set; }
        public int TopicId { get; set; }
        public int ClaimId { get; set; }
        public DateTime DateCreated { get; set; }
        public int UserId { get; set; }
        public string activity_title { get; set; }
    }
}
