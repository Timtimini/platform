namespace Otito.Web.Models.Enum
{
    public enum Gradient
    {
        blue_topic,
        orca_topic,
        green_topic
    }
    public class meta
    {
        private meta(string value) { Value = value; }

        public string Value { get; set; }

        public static meta title { get { return new meta("òtító | a platform for fighting misinformation"); } }
        public static meta description { get { return new meta("Learn the facts about important issues, while contributing evidence to fight misinformation."); } }
        public static meta preamble { get { return new meta("Please share evidence that reveals the truth about this topic - "); } }
        public static meta postfix { get { return new meta("—via Otito platform"); } }

    }
}
