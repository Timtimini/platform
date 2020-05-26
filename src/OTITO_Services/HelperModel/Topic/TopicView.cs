using System;
using System.Collections.Generic;

namespace OTITO_Services.HelperModel.Topic
{
    public class TopicView
    {
        public int Id { get; set; }
        public string TopicName { get; set; }
        public DateTime? DateUpdated { get; set; }
        public DateTime DateCreated { get; set; }
        public IList<ClaimView> Claims { get; set; }
        public bool isSticky { get; set; }
        public DateTime StickedDate { get; set; }
        public int TotalVotes { get; set; }
        public int Contributors { get; set; }
        public string Slug { get; set; }
        public string Guid { get; set; }
    }
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
    public class SourceView
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public string Title { get; set; }
        public int Vote { get; set; }
        public int PositiveKarma { get; set; }
        public int NegativeKarma { get; set; }
        public int TotalVote { get; set; }
        public string Slug { get; set; }
        public string Guid { get; set; }
        public int UserId { get; set; }
    }
}
