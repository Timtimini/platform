using System;
using System.Collections.Generic;

namespace Otito.Web.Models.Topic
{
    public class TopicViewModel
    {
        public int Id { get; set; }
        public string TopicName { get; set; }
        public string GradientName { get; set; }
        public DateTime? DateUpdated { get; set; }
        public DateTime DateCreated { get; set; }
        public IList<ClaimViewModel> Claims { get; set; }
        public AddDispute Dispute { get; set; }
        public bool isSticky { get; set; }
        public string Slug { get; set; }
        public string Guid { get; set; }
        public string search { get; set; }
        public int TotalVotes { get; set; }
        public int Contributors { get; set; }

    }
    public class ClaimViewModel
    {
        public int Id { get; set; }
        public string ClaimTitle { get; set; }
        public string ClaimDescription { get; set; }
        public bool isMostValidated { get; set; }
        public IList<ClaimViewModel> CounterClaims { get; set; }

        public IList<SourceViewModel> Sources { get; set; }
        public string Guid { get; set; }
        public string ParentGuid { get; set; }
        public string Slug { get; set; }
        public int Status { get; set; }
        public int TotalVotes { get; set; }
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        public double KarmaPercent { get; set; }

    }
    public class SourceViewModel
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public string Title { get; set; }
        public int Vote { get; set; }
        public double PositiveKarma { get; set; }
        public double NegativeKarma { get; set; }
        public int TotalVote { get; set; }
        public string Guid { get; set; }
        public string Slug { get; set; }
    }
}
