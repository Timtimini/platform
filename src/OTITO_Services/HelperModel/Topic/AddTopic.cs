using System;
using System.Collections.Generic;

namespace OTITO_Services.HelperModel.Topic
{
    public class AddTopic
    {
        public int Id { get; set; }
        public string TopicName { get; set; }
        public AddClaim Claim { get; set; }
        public string Guid { get; set; }
        public string Slug { get; set; }
    }
    public class AddClaim
    {
        public int Id { get; set; }
        public int ClaimId { get; set; }
        public string ClaimTitle { get; set; }
        public string ClaimDescription { get; set; }
        public IList<AddSource> Sources { get; set; }
        public string ParentGuid { get; set; }
        public string Guid { get; set; }
        public string TopicSlug { get; set; }

    }
    public class AddSource
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public string Guid { get; set; }
    }
}
