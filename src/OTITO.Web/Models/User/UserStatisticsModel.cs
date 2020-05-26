using System;
namespace OTITO.Web.Models.User
{
    public class UserStatisticsModel
    {
        public int NoOfAllUsers { get; set; }
        public int NoOfUsers { get; set; }
        public int NoOfTopics { get; set; }
        public int NoOfClaims { get; set; }
        public int NoOfCounterClaims { get; set; }
        public int NoOfSources { get; set; }
        public int NoOfVotes { get; set; }
        public bool hasData { get; set; }
        public StatsInModel formData { get; set; }
    }
    public class StatsInModel
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
