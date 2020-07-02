namespace Otito.Services.HelperModel.Topic
{
    public class SourceView
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public string Title { get; set; }
        public int Vote { get; set; }
        public int PositiveKarma { get; set; }
        public int NegativeKarma { get; set; }
        public int TotalVote { get; set; }
        public string Slug { get; set; }
        public string Guid { get; set; }
        public int UserId { get; set; }
    }
}