using System.Collections.Generic;

namespace Otito.Services.HelperModel.User
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
}
