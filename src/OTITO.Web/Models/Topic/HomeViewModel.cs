using System;
using System.Collections.Generic;

namespace OTITO.Web.Models.Topic
{
    public class HomeViewModel
    {
        public int CurrentId { get; set; }
        public int PageNo { get; set; }
        public string SearchTerm { get; set; }
        public IList<TopicViewModel> Topics { get; set; }
    }
}
