namespace Otito.Services.HelperModel.Topic
{
    public class AddTopic
    {
        public int Id { get; set; }
        public string TopicName { get; set; }
        public AddClaim Claim { get; set; }
        public string Guid { get; set; }
        public string Slug { get; set; }
    }
}
