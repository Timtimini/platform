using System;
using System.Collections.Generic;

namespace OTITO_Services.Model
{
    public partial class Topic
    {
        public int Id { get; set; }
        public string TopicName { get; set; }
        public DateTime DateCreated { get; set; }
        public int UserId { get; set; }
        public byte? IsDeleted { get; set; }
        public string GradientName { get; set; }
        public byte? IsSticked { get; set; }
        public DateTime? StickedDate { get; set; }
        public string Guid { get; set; }
        public string Slug { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
