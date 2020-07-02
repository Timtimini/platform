using System;
using System.Collections.Generic;
using System.Linq;
using Otito.Services.Model;
using Otito.Services.HelperModel.User;

namespace Otito.Services
{
    public class UserService : IUserService
    {
        private readonly OtitoDbContext _db;

        public UserService(OtitoDbContext db) : base()
        {
            _db = db;
        }

        public User Authenticate(string username, string password)
        {
            var user = _db.User.SingleOrDefault(x => x.Email.Equals(username));

            if (user == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                return null;

            // authentication successful so return user details without password
            user.Password = null;
            return user;
        }

        public User AuthenticateSocial(string email, string firstName, string lastName)
        {
            var user = _db.User.SingleOrDefault(x => x.Email == email);

            if (user == null)
            {
                User _user = new User
                {
                    Email = email,
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

        public int SaveUser(string username, string password)
        {
            var exists = _db.User.FirstOrDefault(x => x.Email.Equals(username));
            if (exists != null)
            {
                return -1;
            }

            var user = new User
            {
                Email = username,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
            };

            _db.User.Add(user);
            _db.SaveChanges();

            return user.Id;
        }

        public Profile GetProfile(int userId)
        {
            var user = _db.User.Where(x => x.Id == userId).Select
            (x => new Profile
                {
                    Id = x.Id,
                    Name = x.Email,
                    IsSocial = Convert.ToInt32(x.IsSocial) == 1,
                    NegativeKarma = x.NegativeKarma ?? 0,
                    PositiveKarma = x.PositiveKarma ?? 0,
                    TotalVote = x.TotalVote
                }
            ).FirstOrDefault();

            var activities = (from ac in _db.Activity
                    join c in _db.Topic on ac.TopicId equals c.Id into t
                    from topic in t.DefaultIfEmpty()
                    join k in _db.Claim on ac.ClaimId equals k.Id into cl
                    from claim in cl.DefaultIfEmpty()
                    where ac.UserId == userId
                    select new ProfileActivity
                    {
                        ClaimId = ac.ClaimId,
                        TopicId = ac.TopicId,
                        activity_type = ac.activity_type,
                        activity_title = ac.activity_title,
                        ClaimSlug = claim.Slug,
                        TopicSlug = topic.Slug,
                        DateCreated = ac.DateCreated
                    }
                ).OrderByDescending(x => x.DateCreated).Take(5).ToList();

            user.activity = activities;


            return user;
        }

        public int ChangePassword(string previous, string newPassword, int userId)
        {
            var user = _db.User.FirstOrDefault(x => x.Id.Equals(userId));

            if (user == null)
                return -1;

            if (!BCrypt.Net.BCrypt.Verify(previous, user.Password))
                return -1;

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _db.SaveChanges();

            return 1;
        }

        public UserStatistics GetStatistics(DateTime from, DateTime to)
        {
            var fromDate = from.Date;
            var toDate = to.Date;
            UserStatistics userStatistics = new UserStatistics
            {
                NoOfUsers = _db.User.Count(),
                NoOfTopics = _db.Topic.Where(x => x.DateCreated.Date >= fromDate && x.DateCreated.Date <= toDate)
                    .Count(),
                NoOfClaims = _db.Claim.Where(x =>
                    x.DateCreated.Date >= fromDate && x.DateCreated.Date <= toDate &&
                    (x.ClaimId == 0 || x.ClaimId == null)).Count(),
                NoOfCounterClaims = _db.Claim.Where(x =>
                    x.DateCreated.Date >= fromDate && x.DateCreated.Date <= toDate && (x.ClaimId > 0)).Count(),
                NoOfSources = _db.Source.Where(x => x.DateCreated.Date >= fromDate && x.DateCreated.Date <= toDate)
                    .Count(),
                NoOfVotes = _db.Vote.Where(x => x.DateCreated.Date >= fromDate && x.DateCreated.Date <= toDate).Count()
            };
            return userStatistics;
        }

        public List<string> UserList()
        {
            var result = _db.User.Select(x => x.Email).ToList();
            return result;
        }

        public bool IfEmailExist(string email)
        {
            var res = _db.User.FirstOrDefault(x => x.Email.Equals(email));
            if (res != null)
                return true;
            return false;
        }
    }
}