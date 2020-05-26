using System;
using OTITO_Services.HelperModel.Topic;
using System.Linq;
using OTITO_Services.Model;
using System.Collections.Generic;

namespace OTITO_Services
{
    public interface IHomeService
    {
        IList<TopicView> GetTopics(int PageNo, int CurrentId, int PageSize, string SearchTerm);
    }
    public class HomeService : IHomeService
    {
        private readonly OtitoDBContext _db;
        public HomeService(OtitoDBContext db) : base()
        {
            _db = db;
        }
        public IList<TopicView> GetTopics(int PageNo, int CurrentId, int PageSize, string SearchTerm)
        {

            var _stickyTopics = (
                                from t in _db.Topic
                                where (PageNo == 1 && SearchTerm == null)
                                && t.IsSticked == 1

                                select new TopicView
                                {
                                    Id = t.Id,
                                    DateUpdated=t.DateUpdated,
                                    TopicName = t.TopicName,
                                    DateCreated = t.DateCreated,
                                    StickedDate = t.StickedDate ?? DateTime.Now,
                                    isSticky = true,
                                    Slug = t.Slug
                                }
                            ).OrderBy(x => x.StickedDate).ToList();
            var _test = _db.Topic.Where(x => SearchTerm == null || x.TopicName.ToLower().Contains(SearchTerm.ToLower())).ToList();
            var _topics = (from t in _db.Topic
                           where (CurrentId == 0 || t.Id > CurrentId) && (SearchTerm == null || t.TopicName.ToLower().Contains(SearchTerm.ToLower()))
                           &&
                           ((!string.IsNullOrEmpty(SearchTerm)) 
                           ||
                           (string.IsNullOrEmpty(SearchTerm) && (t.IsSticked==null || t.IsSticked==0|| t.IsSticked==2))
                           )
                           select new TopicView
                           {
                               Id = t.Id,
                               TopicName = t.TopicName,
                               DateUpdated=t.DateUpdated,
                               DateCreated = t.DateCreated,
                               Slug=t.Slug,
                           }
                         ).OrderByDescending(x=>x.Id).Skip(PageSize*(PageNo-1)).Take(PageNo*PageSize).ToList();


            var _allTopics = (from s in _stickyTopics select s).Union(from t in _topics select t).ToList();

            return _allTopics;
        }


    }


}
