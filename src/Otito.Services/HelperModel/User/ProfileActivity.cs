using System;

namespace Otito.Services.HelperModel.User
{
    public class ProfileActivity
    { 
        public String activity_title { get; set; }
        public String activity_type { get; set; }
        public int TopicId { get; set; }
        public int ClaimId { get; set; }
        public string ClaimSlug { get; set; }
        public string TopicSlug { get; set; }
        public DateTime DateCreated { get; set; }
    }
}