using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using OTITO_Services.Model;
using OTITO_Services.HelperModel.User;
using Microsoft.EntityFrameworkCore;

namespace OTITO_Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        User AuthenticateSocial(string Email, string firstName, string lastName);
        int SaveUser(string Username, string Password);
        Profile GetProfile(int UserId);
        int ChangePassword(string Previous, string NewPassword, int UserId);
        UserStatistics GetStatistics(DateTime from, DateTime to);
        List<string> UserList();
        bool IfEmailExist(string Email);
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        //private List<BasicUser> _users = new List<BasicUser>
        //{
        //    new BasicUser { Id = 1, FirstName = "Test", LastName = "User", Username = "test", Password = "test" }
        //};

        private readonly OtitoDBContext _db;
        public UserService(OtitoDBContext db) : base()
        {
            _db = db;
        }

        public User Authenticate(string username, string password)
        {
            var user = _db.User.SingleOrDefault(x => x.Email == username && x.Password == password
            );

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so return user details without password
            user.Password = null;
            return user;
        }
        public User AuthenticateSocial(string Email, string firstName, string lastName)
        {
            var user = _db.User.SingleOrDefault(x => x.Email == Email);

            if(user==null)
            {
                User _user = new User
                {
                    Email = Email,
                    FirstName = firstName,
                    LastName = lastName,
                    DateCreated = DateTime.Now,
                    IsSocial = 1
                };
                _db.User.Add(_user);
                _db.SaveChanges();
                return _user;
            }
            return user;

        }
        public int SaveUser(string Username, string Password)
        {
            var _exist = _db.User.Where(x => x.Email.Equals(Username)).FirstOrDefault();
            if (_exist != null)
            {
                return -1;
            }
            User user = new User
            {
                Email = Username,
                Password = Password,

            };

            _db.User.Add(user);
            _db.SaveChanges();

            return user.Id;
        }
        public Profile GetProfile(int UserId)
        {
            var user = _db.User.Where(x => x.Id == UserId).Select
              (x=> new Profile
                { 
                   Id=x.Id,
                   Name=x.Email,
                   IsSocial=Convert.ToInt32(x.IsSocial)==1,
                   NegativeKarma=x.NegativeKarma??0,
                   PositiveKarma=x.PositiveKarma??0,
                   TotalVote=x.TotalVote
                }
                ).FirstOrDefault();

            var activities = (from ac in _db.Activity
                      join c in _db.Topic on ac.TopicId equals c.Id into t
                      from topic in t.DefaultIfEmpty()
                      join k in _db.Claim on ac.ClaimId equals k.Id into cl
                      from claim in cl.DefaultIfEmpty()
                      where ac.UserId == UserId
                      select new ProfileActivity
                      {
                          ClaimId = ac.ClaimId,
                          TopicId = ac.TopicId,
                          activity_type = ac.activity_type,
                          activity_title= ac.activity_title,
                          ClaimSlug = claim.Slug,
                          TopicSlug = topic.Slug,
                          DateCreated = ac.DateCreated
                      }
                    ).OrderByDescending(x => x.DateCreated).Take(5).ToList();
            
                 //_db.Activity.Where(x => x.UserId == UserId).OrderByDescending(x => x.DateCreated).Take(5).ToList();
            //var activity = ac
                //.Select(x => new ProfileActivity
                //{
                //    ClaimId = x.ClaimId,
                //    TopicId = x.TopicId,
                //    Title = x.activity_type + " " + x.activity_title
                //}).ToList();

            user.activity = activities;


            return user;
        }
        public int ChangePassword(string Previous, string NewPassword, int UserId)
        {
            var user = _db.User.FirstOrDefault(x => x.Id == UserId && x.Password == Previous);
            if(user!=null)
            {
                user.Password = NewPassword;
                _db.SaveChanges();
                return 1;
            }
            return -1;

        }
        public UserStatistics GetStatistics(DateTime from, DateTime to)
        {
            var fromDate = from.Date;
            var toDate = to.Date;
            UserStatistics userStatistics = new UserStatistics
            {
                NoOfUsers = _db.User.Count(),
                NoOfTopics = _db.Topic.Where(x => x.DateCreated.Date >= fromDate && x.DateCreated.Date <= toDate).Count(),
                NoOfClaims = _db.Claim.Where(x => x.DateCreated.Date >= fromDate && x.DateCreated.Date <= toDate && (x.ClaimId == 0 || x.ClaimId == null)).Count(),
                NoOfCounterClaims = _db.Claim.Where(x => x.DateCreated.Date >= fromDate && x.DateCreated.Date <= toDate && (x.ClaimId > 0)).Count(),
                NoOfSources = _db.Source.Where(x => x.DateCreated.Date >= fromDate && x.DateCreated.Date <= toDate).Count(),
                NoOfVotes = _db.Vote.Where(x => x.DateCreated.Date >= fromDate && x.DateCreated.Date <= toDate).Count()
            };
            return userStatistics;

        }

        public List<string> UserList()
        {
            var result = _db.User.Select(x => x.Email).ToList();
            return result;
        }
        public bool IfEmailExist(string Email)
        {
            var res = _db.User.FirstOrDefault(x => x.Email.Equals(Email));
            if (res != null)
                return true;
            return false;
        }

    }
}
