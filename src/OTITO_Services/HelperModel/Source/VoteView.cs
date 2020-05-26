using System;
using System.Collections.Generic;

namespace OTITO_Services.HelperModel.Source
{
    public class VoteView
    {
       public int ClaimId { get; set; }
       public IList<SourceVote> Votes { get; set; }
    }
    public class SourceVote
    { 
        public int SourceId { get; set; }
        public int Vote { get; set; }
        public double PositiveKarma { get; set; }
        public double NegativeKarma { get; set; }
        public int TotalVote { get; set; }
    }
}
