using System.Collections.Generic;

namespace Otito.Web.Models.Topic
{
    public class TopicModel
    {
        public int Id { get; set; }
        public string TopicName { get; set; }
        public string GradientName { get; set; }
        public ClaimModel Claim { get; set; }
        public string Slug { get; set; }
        public string Guid { get; set; }

    }
    public class ClaimModel
    {
        public int Id { get; set; }
        public string ClaimTitle { get; set; }
        public string ClaimDescription { get; set; }
        public IList<SourceModel> Sources { get; set; }
        public string Slug { get; set; }
        public string ParentGuid { get; set; }
        public string Guid { get; set; }

    }
    public class SourceModel 
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public string Slug { get; set; }
        public string Guid { get; set; }
    }
}
