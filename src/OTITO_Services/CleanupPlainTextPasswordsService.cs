using System.Linq;
using BCrypt.Net;
using OTITO_Services.Model;

namespace OTITO_Services
{
    /// <summary>
    /// Previous developer stored passwords in plaintext
    /// This class will run through all users with a plaintext password and store a hash in its place
    /// Quick and dirty job but meh it's fine for a one time thing.
    /// </summary>
    public class CleanupPlainTextPasswordsService
    {
        private readonly OtitoDBContext _db;

        public CleanupPlainTextPasswordsService(OtitoDBContext db)
        {
            _db = db;
        }

        public void Apply()
        {
            var testUser = _db.User.FirstOrDefault(u => u.Password != null);
            
            if(testUser == null)
                return;

            try
            {
                BCrypt.Net.BCrypt.Verify("foo", testUser.Password);
                // Therefore this has previously run.
            }
            catch (SaltParseException)
            {
                // Cool so we failed to find salt in users password, therefore passwords haven't been hashed yet.

                var users = _db.User.Where(u => u.Password != null);
                foreach (var user in users)
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                }

                _db.SaveChanges();
            }
        }
    }
}