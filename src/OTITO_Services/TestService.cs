using System;
using System.Linq;
using OTITO_Services.Model;

namespace OTITO_Services
{
    public interface ITestService
    {
        User GetUser(int Id);
    }
    public class TestService : ITestService
    {
       private readonly OtitoDBContext _db;
        public TestService(OtitoDBContext db) : base()
        {
            _db = db;
        }
        public User GetUser(int Id)
        {
            var user = _db.User.FirstOrDefault(x=>x.Id==Id);
            return user;
        }
    }
}
