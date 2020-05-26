using System;
using System.Collections.Generic;

namespace OTITO_Services.Model
{
    public partial class Vote
    {
        public int Id { get; set; }
        public int SourceId { get; set; }
        public int UserId { get; set; }
        public int Vote1 { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
