using System;

namespace Otito.Services.Model
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int? PositiveKarma { get; set; }
        public int? NegativeKarma { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int TotalVote { get; set; }
        public string Role { get; set; }
        public DateTime? DateCreated { get; set; }
        public byte? IsSocial { get; set; }
    }
}
