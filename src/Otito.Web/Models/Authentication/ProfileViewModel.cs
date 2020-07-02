using System.Collections.Generic;
using System.Security.Claims;

namespace Otito.Web.Models.Authentication
{
    public class ProfileViewModel
    {
        public IEnumerable<Claim> Claims { get; set; }
        public string Name { get; set; }
    }
}
