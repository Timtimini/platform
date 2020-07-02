namespace Otito.Services.HelperModel.Source
{
    public class SourceVote
    { 
        public int SourceId { get; set; }
        public int Vote { get; set; }
        public double PositiveKarma { get; set; }
        public double NegativeKarma { get; set; }
        public int TotalVote { get; set; }
    }
}