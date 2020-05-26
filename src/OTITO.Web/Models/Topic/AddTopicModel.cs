using System;
using System.Collections.Generic;

namespace OTITO.Web.Models.Topic
{
    public class AddTopicModel
    {
        public int TopicId { get; set; }
        public string TopicName { get; set; }
        public AddClaimModel Claim { get; set; }
        public string Slug { get; set; }
        public string Guid { get; set; }
    }
    public class AddClaimModel
    {
        public int TopicId { get; set; }
        public int Id { get; set; }
        public int ClaimId { get; set; }
        public string ClaimTitle { get; set; }
        public string ClaimDescription { get; set; }
        public IList<AddSourceModel> Sources { get; set; }
        public string Slug { get; set; }
        public string Guid { get; set; }
    }
    public class AddSourceModel
    {

        public string URL { get; set; }
        public string Slug { get; set; }
        public string Guid { get; set; }
    }
}
