using System;
using System.Collections.Generic;

namespace OTITO_Services.Model
{
    public partial class Source
    {
        public int Id { get; set; }
        public int ClaimId { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime DateCreated { get; set; }
        public byte? IsDeleted { get; set; }
        public int? UpVote { get; set; }
        public int? DownVote { get; set; }
        public int? Vote { get; set; }
        public int UserId { get; set; }
        public string Guid { get; set; }
        public string Slug { get; set; }
    }
}
