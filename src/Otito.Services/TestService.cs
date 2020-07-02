using System.Linq;
using Otito.Services.Model;

namespace Otito.Services
{
    public interface ITestService
    {
        User GetUser(int Id);
    }
    public class TestService : ITestService
    {
       private readonly OtitoDbContext _db;
        public TestService(OtitoDbContext db) : base()
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
