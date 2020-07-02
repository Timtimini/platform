using System.ComponentModel.DataAnnotations;

namespace Otito.Web.Models.Topic
{
    public class AddDispute
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Source { get; set; }
        public int TopicId { get; set; }
        public int ClaimId { get; set; }
        public int SourceId { get; set; }
        public string SourceTitle { get; set; }
        public string SourceURL { get; set; }
        public string ClaimSlug { get; set; }
        public string TopicSlug { get; set; }
        public string SourceSlug { get; set; }
    }

}
