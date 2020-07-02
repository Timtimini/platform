using System.Collections.Generic;

namespace Otito.Services.HelperModel.Topic
{
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
}