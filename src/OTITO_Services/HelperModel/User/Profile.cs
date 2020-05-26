using System;
using System.Collections.Generic;

namespace OTITO_Services.HelperModel.User
{
    public class Profile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PositiveKarma { get; set; }
        public int NegativeKarma { get; set; }
        public int TotalVote { get; set; }
        public IList<ProfileActivity> activity { get; set; }
        public bool IsSocial { get; set; }
    }
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
