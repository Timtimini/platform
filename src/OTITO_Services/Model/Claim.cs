using System;
using System.Collections.Generic;

namespace OTITO_Services.Model
{
    public partial class Claim
    {
        public int Id { get; set; }
        public string ClaimTitle { get; set; }
        public string ClaimDescription { get; set; }
        public DateTime DateCreated { get; set; }
        public int UserId { get; set; }
        public int? ClaimId { get; set; }
        public int? TopicId { get; set; }
        public byte? IsDeleted { get; set; }
        public int? Sources { get; set; }
        public int? Vote { get; set; }
        public string ClaimGuid { get; set; }
        public string Guid { get; set; }
        public string Slug { get; set; }
    }
}
