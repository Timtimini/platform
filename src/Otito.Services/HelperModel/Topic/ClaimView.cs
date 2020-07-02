using System.Collections.Generic;

namespace Otito.Services.HelperModel.Topic
{
    public class ClaimView
    {
        public int Id { get; set; }
        public string ClaimTitle { get; set; }
        public string ClaimDescription { get; set; }
        public int ClaimId { get; set; }
        public int TotalVotes { get; set; }
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        public IList<ClaimView> CounterClaims { get; set; }
        public IList<SourceView> Sources { get; set; }
        public string ParentGuid { get; set; }
        public string Slug { get; set; }
        public string Guid { get; set; }
        public int Status { get; set; }
        public int UserId { get; set; }
        public int SourceContributors { get; set; }
    }
}