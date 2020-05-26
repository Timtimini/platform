using System;
using System.Collections.Generic;

namespace OTITO.Web.Models.User
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double PositiveKarma { get; set; }
        public double NegativeKarma { get; set; }
        public IList<UserProfileActivity> Activity { get; set; }
        public bool IsSocial { get; set; }
    }
    public class UserProfileActivity
    {
        public string Title { get; set; }
        public int TopicId { get; set; }
        public int ClaimId { get; set; }

        public string URL { get; set; }
    }
}
