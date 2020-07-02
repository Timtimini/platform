using System.Collections.Generic;
using Otito.Services.HelperModel.Topic;

namespace Otito.Services
{
    public interface IHomeService
    {
        IList<TopicView> GetTopics(int PageNo, int CurrentId, int PageSize, string SearchTerm);
    }
}