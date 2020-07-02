using System;

namespace Otito.Services.Model
{
    public class Vote
    {
        public int Id { get; set; }
        public int SourceId { get; set; }
        public int UserId { get; set; }
        public int Vote1 { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
