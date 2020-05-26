using System;
using OTITO_Services.HelperModel.Topic;
using System.Linq;
using OTITO_Services.Model;
using OTITO_Services.HelperModel.Source;
using System.Collections.Generic;
using OTITO_Services.HelperModel.Slug;

namespace OTITO_Services
{
    public interface ITopicService
    {
        string SaveTopic(AddTopic topic, int UserId);
        int SaveClaim(AddClaim claim, int TopicId, int UserId);
        string SaveSource(string Guid, string Slug, string SlugWithoutGuid, string Source, int UserId);
        Topic GetTopic(string slug);
        TopicView TopicSimpleDetail(string slug);
        TopicView TopicEditorDetail(string slug);
        TopicView TopicEditorDetail(string slug, string search);
        TopicView ClaimDetail(int ClaimId, int TopicId);
        TopicView ClaimDetailSource(string slug);
        AddVote AddVote(int SourceId, int ClaimId, int TopicId, int Vote, int UserId);
        IList<SourceVote> GetVotes(int ClaimId);
        void DeleteTopic(string slug);
        void DeleteSource(string slug);
        void DeleteClaim(string slug);
        TopicView SourceDispute(string SourceSlug);
        void addSticky(string slug);
        void removeSticky(string slug);
        void addSlugs();
    }
    public class TopicService : ITopicService
    {
        private readonly OtitoDBContext _db;
        public TopicService(OtitoDBContext db) : base()
        {
            _db = db;
        }
        public string SaveTopic(AddTopic topic,int UserId)
        {
            Topic _topic = new Topic
            {
                TopicName = topic.TopicName,
                DateCreated = DateTime.Now,
                UserId=UserId,


            };
            _db.Topic.Add(_topic);
            _db.SaveChanges();
            _topic.Guid = topic.Guid + _topic.Id;
            _topic.Slug = topic.Slug + "-" + _topic.Guid;
            _db.SaveChanges();
            Claim _claim = new Claim
            {
                ClaimTitle = topic.Claim.ClaimTitle,
                ClaimDescription = topic.Claim.ClaimDescription,
                DateCreated = DateTime.Now,
                IsDeleted = 0,
                TopicId = _topic.Id,
                UserId = UserId,
                Sources= topic.Claim.Sources.Count(),

            };
            _claim.Sources = topic.Claim.Sources.Count();

            _db.Claim.Add(_claim);
            _db.SaveChanges();
            _claim.Guid = topic.Claim.Guid + _claim.Id;
            _claim.Slug = _topic.Slug + "-" + _claim.Guid;
            int i = 1;
            foreach (var item in topic.Claim.Sources)
            {
                Source _source = new Source
                {
                    DateCreated = DateTime.Now,
                    IsDeleted = 0,
                    Title = "Source " + i,
                    ClaimId = _claim.Id,
                    UserId=UserId,
                    Url = item.URL
                };

                _db.Source.Add(_source);
                _db.SaveChanges();
                _source.Guid = item.Guid + _source.Id;
                _source.Slug = _topic.Slug + "-" + _source.Guid;
                i++;
            }

            Activity activity = new Activity
            {
                UserId = UserId,
                DateCreated = DateTime.Now,
                activity_title = topic.TopicName,
                activity_type = "Topic Added: ",
                TopicId=_topic.Id
            };
            _db.Activity.Add(activity);

            _db.SaveChanges();
            return _topic.Slug;
        }

        public int SaveClaim(AddClaim claim, int TopicId, int UserId)
        {

            var parentClaim = _db.Claim.FirstOrDefault(x => x.Guid == claim.ParentGuid);
            var topic = _db.Topic.FirstOrDefault(x => x.Id == TopicId);
            topic.DateUpdated = DateTime.Now;

            Claim _claim = new Claim
            {
                ClaimTitle = claim.ClaimTitle,
                ClaimDescription = claim.ClaimDescription,
                DateCreated = DateTime.Now,

                IsDeleted = 0,
                ClaimId= parentClaim==null? 0: parentClaim.Id,
                TopicId = TopicId,
                UserId = UserId,
                ClaimGuid=claim.ParentGuid,

            };
            _db.Claim.Add(_claim);
            _db.SaveChanges();
            _claim.Guid = claim.Guid + _claim.Id;
            _claim.Slug = claim.TopicSlug + _claim.Guid;


            int i = 1;
            foreach (var item in claim.Sources)
            {
                Source _source = new Source
                {
                    DateCreated = DateTime.Now,
                    IsDeleted = 0,
                    Title = "Source " + i,
                    ClaimId = _claim.Id,
                    UserId=UserId,
                    Url = item.URL,

                };

                _db.Source.Add(_source);
                _db.SaveChanges();
                _source.Guid = item.Guid + _source.Id;
                _source.Slug = claim.TopicSlug + _source.Guid;

                i++;
            }
            _claim.Sources = claim.Sources.Count();

            Activity activity = new Activity
            {
                UserId = UserId,
                DateCreated = DateTime.Now,
                activity_title = claim.ClaimTitle,
                activity_type = "Claim Added: ",
                TopicId=TopicId,
                ClaimId= _claim.Id
            };
            _db.Activity.Add(activity);

            _db.SaveChanges();
            return _claim.Id;
        }

        public Topic GetTopic(string slug)
        {
            var _topic = _db.Topic.Where(x => x.Slug.Equals(slug)).Select(v => new Topic { TopicName = v.TopicName, Slug=v.Slug,
                    Guid=v.Guid,
                    Id=v.Id,
                      }).FirstOrDefault();
            return _topic;
        }
        public TopicView TopicSimpleDetail(string slug)
        {
            var _topic = (from t in _db.Topic
                          where t.Slug.Equals(slug)
                          select new TopicView
                          {
                              Id = t.Id,
                              TopicName = t.TopicName,
                              isSticky=Convert.ToInt32(t.IsSticked)==1?true:false,
                              Slug=t.Slug,
                              Guid=t.Guid,
                              Claims = (
                               from c in _db.Claim.Where(x => x.TopicId == t.Id && (x.ClaimId==null || x.ClaimId==0))
                               select new ClaimView
                               {
                                   Id = c.Id,
                                   ClaimDescription = c.ClaimDescription,
                                   ClaimTitle = c.ClaimTitle,
                                   TotalVotes=c.Vote??0,
                                   Guid=c.Guid,
                                   Slug=c.Slug,
                                   Status=_db.Claim.Any(x=>x.ClaimId==c.Id)?1:2,
                                   CounterClaims = (
                                                   from cc in _db.Claim.Where(x => x.ClaimId == c.Id)
                                                   select new ClaimView
                                                   {
                                                       Id = cc.Id,
                                                       ClaimDescription = cc.ClaimDescription,
                                                       TotalVotes=cc.Vote??0,
                                                       ClaimTitle = cc.ClaimTitle,
                                                       Slug=cc.Slug,
                                                       Guid=cc.Guid,
                                                   }
                                                  ).OrderByDescending(x => x.TotalVotes).ToList()

                               }
                              ).OrderByDescending(x=>x.TotalVotes).ToList()
                          }).FirstOrDefault();

            return _topic;

        }
        public TopicView TopicEditorDetail(string slug)
        {
            var _topic = (from t in _db.Topic
                          where t.Slug.Equals(slug)
                          select new TopicView
                          {
                              Id = t.Id,
                              TopicName = t.TopicName,
                              Slug=t.Slug,
                              Guid=t.Guid,
                              DateCreated=t.DateCreated,
                              Claims = (
                               from c in _db.Claim.Where(x => x.TopicId == t.Id && (x.ClaimId == null || x.ClaimId == 0))
                               select new ClaimView
                               {
                                   Id = c.Id,
                                   ClaimDescription = c.ClaimDescription,
                                   ClaimTitle = c.ClaimTitle,
                                   TotalVotes=c.Vote??0,
                                   Guid=c.Guid,
                                   Slug=c.Slug,
                                   Status=_db.Claim.Any(x=>x.ClaimId==c.Id)?1:2,
                                   CounterClaims = (
                                                   from cc in _db.Claim.Where(x => x.ClaimId == c.Id)
                                                   select new ClaimView
                                                   {
                                                       Id = cc.Id,
                                                       ClaimDescription = cc.ClaimDescription,
                                                       ClaimTitle = cc.ClaimTitle,
                                                       ParentGuid=cc.ClaimGuid,
                                                       TotalVotes=cc.Vote??0,
                                                       Slug=cc.Slug,
                                                       Guid=cc.Guid
                                                   }
                                                  ).OrderByDescending(x => x.TotalVotes).ToList()
                               }
                              ).OrderByDescending(x => x.TotalVotes).ToList()
                          }).FirstOrDefault();

            return _topic;
        }
        public TopicView TopicEditorDetail(string slug, string search)
        {
            var topicId = _db.Topic.Where(x => x.Slug.Equals(slug));

            var claimUser = _db.Claim.Where(x => x.TopicId == topicId.FirstOrDefault().Id);
            var claimIds = claimUser.Select(x => x.Id).ToList();
            var claimCount = claimUser.Select(x => x.UserId);

            var sourceUser = _db.Source.Where(x => claimIds.Contains(x.ClaimId)).ToList();//.Select(x=>x.UserId)
            var sourceIds = sourceUser.Select(x => x.Id).ToList();

            var sourceCount = sourceUser.Select(x => x.UserId).ToList();

            var voteCount = _db.Vote.Where(x => sourceIds.Contains(x.SourceId));//.Select(x=>x.UserId).ToList();

            var totals = (from t in topicId select t.UserId).Union
            (from c in claimUser select c.UserId).Union(
                          from s in sourceUser select s.UserId).Union(
                          from v in voteCount select v.UserId).Select(x=>x).Distinct().Count();





            var _topic = (from t in _db.Topic
                          where t.Slug.Equals(slug)
                          select new TopicView
                          {
                              Id = t.Id,
                              TopicName = t.TopicName,
                              Slug = t.Slug,
                              Guid = t.Guid,
                              Contributors=totals,
                              DateCreated = t.DateCreated,
                              Claims = (
                               from c in _db.Claim.Where(x => x.TopicId == t.Id && (x.ClaimId == null || x.ClaimId == 0)
                              && (search == null || (x.ClaimTitle.Contains(search) || x.ClaimDescription.Contains(search)))
                               )
                               select new ClaimView
                               {
                                   Id = c.Id,
                                   ClaimDescription = c.ClaimDescription,
                                   ClaimTitle = c.ClaimTitle,
                                   TotalVotes = c.Vote ?? 0,
                                   UserId=c.UserId,
                                   Guid = c.Guid,
                                   Slug = c.Slug,
                                   UpVotes = _db.Source.Where(x => x.ClaimId == c.Id).Select(x => new { value = x.UpVote ?? 0 }).Sum(x => x.value),
                                   DownVotes = _db.Source.Where(x => x.ClaimId == c.Id).Select(x => new { value = x.DownVote ?? 0 }).Sum(x => x.value),
                                   SourceContributors = _db.Source.Select(x => new { UserId = x.UserId }).Distinct().Count(),
                                   //1=>Most Validated
                                   //2=> UnChallenged
                                   Status =_db.Claim.Any(x=>x.ClaimId==c.Id)?1:2,
                                   CounterClaims = (
                                                   from cc in _db.Claim.Where(x => x.ClaimId == c.Id
                                                    && (search == null || (x.ClaimTitle.Contains(search) || x.ClaimDescription.Contains(search)))
                                                   )
                                                   select new ClaimView
                                                   {
                                                       Id = cc.Id,
                                                       ClaimDescription = cc.ClaimDescription,
                                                       TotalVotes=cc.Vote??0,
                                                       ClaimTitle = cc.ClaimTitle,
                                                       ParentGuid = cc.ClaimGuid,
                                                       UpVotes=_db.Source.Where(x=>x.ClaimId==cc.Id).Select(x=> new { value = x.UpVote ?? 0 }).Sum(x=>x.value),
                                                       DownVotes= _db.Source.Where(x => x.ClaimId == cc.Id).Select(x => new { value = x.DownVote ?? 0 }).Sum(x => x.value),
                                                       Slug = cc.Slug,
                                                       Guid = cc.Guid,
                                                       UserId = cc.UserId,
                                                       SourceContributors = _db.Source.Select(x => new { UserId = x.UserId }).Distinct().Count(),
                                                   }
                                                  ).OrderByDescending(x => x.TotalVotes).ToList()
                               }
                              ).OrderByDescending(x => x.TotalVotes).ToList()
                          }).FirstOrDefault();
            _topic.TotalVotes = (
                                from c in _topic.Claims
                                select new
                                {
                                    Total = c.TotalVotes,
                                    TotalNested = c.CounterClaims.Sum(x=>x.TotalVotes)
                                }
                                ).Sum(x=>x.Total+x.TotalNested);
            return _topic;
        }
        public TopicView ClaimDetail(int ClaimId, int TopicId)
        {
            var _topic = (from t in _db.Topic
                          where t.Id == TopicId
                          select new TopicView
                          {
                              Id = t.Id,
                              TopicName = t.TopicName,

                              Claims = (
                               from c in _db.Claim.Where(x => x.Id == ClaimId)
                               select new ClaimView
                               {
                                   Id = c.Id,
                                   ClaimDescription = c.ClaimDescription,
                                   ClaimTitle = c.ClaimTitle,
                                   ClaimId = c.ClaimId??0,
                                   CounterClaims = (
                                                   from cc in _db.Claim.Where(x => x.ClaimId == c.Id)
                                                   select new ClaimView
                                                   {
                                                       Id = cc.Id,
                                                       ClaimDescription = cc.ClaimDescription,
                                                       ClaimTitle = cc.ClaimTitle,

                                                   }
                                                  ).ToList(),
                                   Sources =(from s in _db.Source.Where(v=>v.ClaimId==ClaimId)
                                            select new SourceView
                                            { 
                                                Id=s.Id,
                                                Title=s.Title,
                                                URL=s.Url,
                                                Vote=s.Vote??0
                                            }
                                            ).ToList()

                               }
                              ).ToList()
                          }).FirstOrDefault();

            return _topic;
        }
        public TopicView ClaimDetailSource(string slug)
        {
            var _claim = (from c in _db.Claim
                          where c.Slug.Equals(slug)
                          select c
                          ).FirstOrDefault();
            //_db.Claim.Where(x => x.Id == 42);
            //if(_claim!=null)
            //{
            var _topic = (from t in _db.Topic
                          where t.Id == _claim.TopicId
                          select new TopicView
                          {
                              Id = t.Id,
                              TopicName = t.TopicName,
                              Guid = t.Guid,
                              Slug = t.Slug,
                              Claims = (
                               from c in _db.Claim.Where(x => x.Id == _claim.Id)
                               select new ClaimView
                               {
                                   Id = c.Id,
                                   ClaimDescription = c.ClaimDescription,
                                   ClaimTitle = c.ClaimTitle,
                                   ClaimId = c.ClaimId ?? 0,
                                   Guid = c.Guid,
                                   Slug = c.Slug,
                                   Status = _db.Claim.Any(x => x.ClaimId == c.Id) ? 1 : 2,
                                       CounterClaims = (
                                                       from cc in _db.Claim.Where(x => x.ClaimId == c.Id)
                                                       select new ClaimView
                                                       {
                                                           Id = cc.Id,
                                                           ClaimDescription = cc.ClaimDescription,
                                                           ClaimTitle = cc.ClaimTitle,
                                                           ParentGuid = cc.ClaimGuid,
                                                           Guid = cc.Guid,
                                                           Slug = cc.Slug

                                                       }
                                                      ).ToList(),
                                       Sources = (from s in _db.Source.Where(v => v.ClaimId == c.Id)
                                                  join u in _db.User on s.UserId equals u.Id
                                                  select new SourceView
                                                  {
                                                      Id = s.Id,
                                                      Title = s.Title,
                                                      URL = s.Url,
                                                      Vote = s.Vote ?? 0,
                                                      NegativeKarma = u.NegativeKarma ?? 0,
                                                      PositiveKarma = u.PositiveKarma ?? 0,
                                                      TotalVote = u.TotalVote,
                                                      Slug = s.Slug,
                                                      Guid = s.Guid
                                                  }
                                                ).ToList()

                                   }
                                  ).ToList()
                              }).FirstOrDefault();
                return _topic;
            //}
           // return null;


        }
        public AddVote AddVote(int SourceId, int ClaimId, int TopicId, int Vote, int UserId)
        {
            var topic = _db.Topic.FirstOrDefault(x => x.Id == TopicId);
            topic.DateUpdated = DateTime.Now;

            var exist = _db.Vote.FirstOrDefault(x => x.UserId == UserId && x.SourceId == SourceId);
            if(exist!=null && exist.Vote1==Vote)
            {
                AddVote _vote = new AddVote
                {
                    Vote = 0,
                    success = false
                };
                return _vote;
            }
            if (exist != null)
            {
                exist.Vote1 = Vote;
                Vote = Vote * 2;

                var _source = _db.Source.FirstOrDefault(x => x.Id == SourceId);
                _source.Vote = (_source.Vote ?? 0) + Vote;
                _db.SaveChanges();

                var claim_pc = (from c in _db.Claim
                                join cc in _db.Claim on c.ClaimId equals cc.Id into pc

                                from oc in pc.DefaultIfEmpty()
                                where c.Id == ClaimId
                                select new
                                {
                                    c,
                                    oc
                                }
                                ).FirstOrDefault();
               // claim_pc.c.Sources = (claim_pc.c.Sources ?? 0) + 1;
                claim_pc.c.Vote = (claim_pc.c.Vote ?? 0) + Vote;

                if (claim_pc.oc != null)
                {
                    if ((claim_pc.c.Vote ?? 0) > (claim_pc.oc.Vote ?? 0))
                    {
                        //do the rest
                        var AllClaims = _db.Claim.Where(x => x.TopicId == TopicId && x.ClaimId == claim_pc.oc.Id
                        && x.Id != ClaimId
                        ).ToList();

                        claim_pc.c.ClaimId = null;
                        claim_pc.oc.ClaimId = claim_pc.c.Id;
                        foreach (var item in AllClaims)
                        {
                            item.ClaimId = claim_pc.c.Id;
                        }

                    }
                }

               

                var user = _db.User.FirstOrDefault(x => x.Id == _source.UserId);
                if (user != null)
                {
                    if (Vote == -2)
                    {
                        user.PositiveKarma = (user.PositiveKarma ?? 0) - 1;
                        user.NegativeKarma = (user.NegativeKarma ?? 0) + 1;
                    }
                    else
                    {
                        user.PositiveKarma = (user.PositiveKarma ?? 0) + 1;
                        user.NegativeKarma = (user.NegativeKarma ?? 0) - 1;
                    }

                    //user.TotalVote = user.TotalVote + 1;
                }

                //Activity activity = new Activity
                //{
                //    UserId = UserId,
                //    DateCreated = DateTime.Now,
                //    activity_title = claim_pc.c.ClaimTitle,
                //    activity_type = "Voted: ",
                //    TopicId = TopicId,
                //    ClaimId = ClaimId
                //};
                //_db.Activity.Add(activity);

                _db.SaveChanges();

                AddVote _vote = new AddVote
                {
                    Vote = _source.Vote ?? 0,
                    success = true
                };
                return _vote;



            }
            else
            {

                var _source = _db.Source.FirstOrDefault(x => x.Id == SourceId);
                _source.Vote = (_source.Vote ?? 0) + Vote;
                _db.SaveChanges();

                var claim_pc = (from c in _db.Claim
                                join cc in _db.Claim on c.ClaimId equals cc.Id into pc

                                from oc in pc.DefaultIfEmpty()
                                where c.Id == ClaimId
                                select new
                                {
                                    c,
                                    oc
                                }
                                ).FirstOrDefault();
                //claim_pc.c.Sources = (claim_pc.c.Sources ?? 0) + 1;
                claim_pc.c.Vote = (claim_pc.c.Vote ?? 0) + Vote;

                if (claim_pc.oc != null)
                {
                    if ((claim_pc.c.Vote ?? 0) > (claim_pc.oc.Vote ?? 0))
                    {
                        //do the rest
                        var AllClaims = _db.Claim.Where(x => x.TopicId == TopicId && x.ClaimId == claim_pc.oc.Id
                        && x.Id != ClaimId
                        ).ToList();

                        claim_pc.c.ClaimId = null;
                        claim_pc.oc.ClaimId = claim_pc.c.Id;
                        foreach (var item in AllClaims)
                        {
                            item.ClaimId = claim_pc.c.Id;
                        }

                    }
                }

                Vote _vote = new Vote
                {
                    SourceId = SourceId,
                    UserId = UserId,
                    DateCreated = DateTime.Now,
                    Vote1 = Vote
                };
                _db.Vote.Add(_vote);

                var user = _db.User.FirstOrDefault(x => x.Id == _source.UserId);
                if (user != null)
                {
                    if (Vote == -1)
                    {
                        user.NegativeKarma = (user.NegativeKarma ?? 0) + 1;
                    }
                    else
                    {
                        user.PositiveKarma = (user.PositiveKarma ?? 0) + 1;
                    }

                    user.TotalVote = user.TotalVote + 1;
                }

                Activity activity = new Activity
                {
                    UserId = UserId,
                    DateCreated = DateTime.Now,
                    activity_title = claim_pc.c.ClaimTitle,
                    activity_type = "Voted: ",
                    TopicId = TopicId,
                    ClaimId = ClaimId
                };
                _db.Activity.Add(activity);

                _db.SaveChanges();

                AddVote _v = new AddVote
                {
                    Vote = _source.Vote ?? 0,
                    success = true
                };
                return _v;
            }
        }
        public IList<SourceVote> GetVotes(int ClaimId)
        {
            var _data = ( from v in _db.Source.Where(x => x.ClaimId == ClaimId)
                          join u in _db.User on v.UserId equals u.Id
                         
                    select new SourceVote
                    {
                        SourceId = v.Id,
                        Vote = v.Vote ?? 0,
                        NegativeKarma = (u.TotalVote == 0 ? 0 : (double)(u.NegativeKarma??0) / (double)(u.TotalVote))*100,
                        PositiveKarma = (u.TotalVote == 0 ? 0 : (double)(u.PositiveKarma??0) / (double)(u.TotalVote))*100,

                    }
                ).ToList();
           
            return _data;
        }
        public string SaveSource(string Guid,string Slug, string SlugWithoutGuid, string Source, int UserId)
        {

            var _claim = _db.Claim.FirstOrDefault(x => x.Slug.Equals(Slug));
            var sourceCount = _db.Source.Where(x => x.ClaimId == _claim.Id).Count();
            Source source = new Source
            {
                Title = "Source " + (sourceCount + 1),
                ClaimId = _claim.Id,
                DateCreated=DateTime.Now,
                Url = Source,
                UserId= UserId,

            };

            _db.Source.Add(source);
            _db.SaveChanges();
            source.Guid = Guid + source.Id;
            source.Slug = SlugWithoutGuid + source.Guid;
             
            _claim.Sources = (_claim.Sources ?? 0) + 1;

            Activity activity = new Activity
            {
                UserId = UserId,
                DateCreated = DateTime.Now,
                activity_title = _claim.ClaimTitle,
                activity_type = "Source Added: ",
                TopicId= _claim.TopicId??0,
                ClaimId = _claim.Id
            };
            _db.Activity.Add(activity);

            var topic = _db.Topic.FirstOrDefault(x => x.Id == _claim.TopicId);
            topic.DateUpdated = DateTime.Now;

            _db.SaveChanges();
            return source.Slug;
        }
        public void DeleteTopic(string slug)
        {
            var topic = _db.Topic.Where(x => x.Slug.Equals(slug)).FirstOrDefault();
            topic.DateUpdated = DateTime.Now;
            var claims = _db.Claim.Where(x => x.TopicId == topic.Id).ToList();

            var claimIds = claims.Select(x => x.Id).ToList();
            var sources = _db.Source.Where(x => claimIds.Contains(x.ClaimId));
            _db.Source.RemoveRange(sources);
            _db.Claim.RemoveRange(claims);
            _db.Topic.Remove(topic);
            _db.SaveChanges();
        }
        public void DeleteSource(string slug)
        {
            var source = _db.Source.FirstOrDefault(x => x.Slug.Equals(slug));
            if(source!=null)
            {
               

                var claim_pc = (from Parent in _db.Claim
                                join cc in _db.Claim on Parent.Id equals cc.ClaimId into pc

                                from Child in pc.DefaultIfEmpty()
                                where Parent.Id == source.ClaimId
                                select new
                                {
                                    Parent,
                                    Child
                                }
                                ).FirstOrDefault();
                var topic = _db.Topic.FirstOrDefault(x => x.Id == claim_pc.Parent.TopicId);
                topic.DateUpdated = DateTime.Now;

                claim_pc.Parent.Sources = (claim_pc.Parent.Sources ?? 0) - 1;
                claim_pc.Parent.Vote = claim_pc.Parent.Vote - (source.Vote ?? 0);

                if (claim_pc.Child != null)
                {
                    if ((claim_pc.Child.Vote ?? 0) > (claim_pc.Parent.Vote ?? 0))
                    {
                        //do the rest
                        var AllClaims = _db.Claim.Where(x => x.TopicId ==  claim_pc.Parent.TopicId && x.ClaimId == claim_pc.Parent.Id
                        && x.Id != source.ClaimId
                        ).ToList();

                        claim_pc.Child.ClaimId = null;
                        claim_pc.Parent.ClaimId = claim_pc.Child.Id;
                        foreach (var item in AllClaims)
                        {
                            if(claim_pc.Child.ClaimId!=item.ClaimId)
                            {
                                item.ClaimId = claim_pc.Child.Id;
                            }

                        }

                    }
                }

                _db.Source.Remove(source);
                _db.SaveChanges();
            }
        }
        public void DeleteClaim(string slug)
        {
            var claimToDelete = _db.Claim.FirstOrDefault(x => x.Slug == slug);
            var topic = _db.Topic.FirstOrDefault(x => x.Id == claimToDelete.TopicId);
            topic.DateUpdated = DateTime.Now;
            var sources = _db.Source.Where(x => x.ClaimId == claimToDelete.Id);
            //var claim_pc = (from c in _db.Claim
            //join cc in _db.Claim on c.Id equals cc.ClaimId into pc

            //from oc in pc.DefaultIfEmpty()
            //where c.Id == ClaimId
            //select new
            //{
            //    c,
            //    oc
            //}
            //).FirstOrDefault();
            //var claimToDelete = _db.Claim.FirstOrDefault(x => x.Id == claim.Id);


            if (claimToDelete != null)
            {

                    //do the rest
                    var AllClaims = _db.Claim.Where(x => x.TopicId == claimToDelete.TopicId
                    && x.Id != claimToDelete.Id && x.ClaimId==claimToDelete.Id
                    ).ToList();

                var master = AllClaims.OrderByDescending(x => x.Vote).FirstOrDefault();
                if(master!=null)
                master.ClaimId = null;
                    foreach (var item in AllClaims)
                    {
                    if(master.ClaimId!=item.ClaimId)
                        item.ClaimId = master.Id;
                    }

               
            }

            _db.Source.RemoveRange(sources);
            _db.Claim.Remove(claimToDelete);
            _db.SaveChanges();

        }
        public TopicView SourceDispute(string SourceSlug)
        {
            var source = _db.Source.FirstOrDefault(x => x.Slug.Equals(SourceSlug));
            var claim = _db.Claim.FirstOrDefault(x => x.Id == source.ClaimId);
            var _topic = (from t in _db.Topic
                          where t.Id == claim.TopicId
                          select new TopicView
                          {
                              Id = t.Id,
                              TopicName = t.TopicName,
                              Slug=t.Slug,
                              Guid=t.Guid,
                              Claims = (
                               from c in _db.Claim.Where(x => x.Id == claim.Id)
                               select new ClaimView
                               {
                                   Id = c.Id,
                                   ClaimDescription = c.ClaimDescription,
                                   ClaimTitle = c.ClaimTitle,
                                   Slug=c.Slug,
                                   Guid=c.Guid,
                                   Sources = (from s in _db.Source.Where(v => v.ClaimId == claim.Id && v.Id==source.Id)
                                              select new SourceView
                                              {
                                                  Id = s.Id,
                                                  Title = s.Title,
                                                  URL = s.Url,
                                                  Vote = s.Vote ?? 0,
                                                  Slug=s.Slug,
                                                  Guid=s.Guid
                                              }
                                            ).ToList()

                               }
                              ).ToList()
                          }).FirstOrDefault();

            return _topic;
        }
        public void addSticky(string slug)
        {
            var _topic = _db.Topic.FirstOrDefault(x => x.Slug.Equals(slug));
            if(_topic!=null)
            {
                _topic.IsSticked = 1;
                _topic.StickedDate = DateTime.Now; 
            }
            _db.SaveChanges();
        }
        public void removeSticky(string slug)
        {
            var _topic = _db.Topic.FirstOrDefault(x => x.Slug.Equals(slug));
            if(_topic!=null)
            {
                _topic.IsSticked = 2;
            }
            _db.SaveChanges();
        }
        public void addSlugs()
        {
            var _topics = _db.Topic.ToList();
            foreach (var topic in _topics)
            {
                var topicSlug = Slug.ToUrlSlug(topic.TopicName);
                string topicGuid = Slug.ToNewGuid();
                topic.Guid = topicGuid + topic.Id;
                topic.Slug = topicSlug+"-"+topic.Guid; 
                var _claims = _db.Claim.Where(x => x.TopicId==topic.Id).ToList();

                foreach (var claim in _claims)
                {
                    string claimGuid = Slug.ToNewGuid();
                    claim.Guid = claimGuid + topic.Id;
                    claim.Slug = topicSlug +"-"+ claim.Guid;

                    var _sources = _db.Source.Where(x => x.ClaimId == claim.Id);

                    foreach (var source in _sources)
                    {
                        string sourceGuid = Slug.ToNewGuid();
                        source.Guid = sourceGuid + source.Id;
                        source.Slug = topicSlug +"-"+ source.Guid;

                    }

                }

            }

            _db.SaveChanges();
        }
    }
}
