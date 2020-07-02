using System.Collections.Generic;

namespace Otito.Services.HelperModel.Source
{
    public class VoteView
    {
       public int ClaimId { get; set; }
       public IList<SourceVote> Votes { get; set; }
    }
}
