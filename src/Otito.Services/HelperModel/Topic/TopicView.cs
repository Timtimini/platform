using System;
using System.Collections.Generic;

namespace Otito.Services.HelperModel.Topic
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
}
