using Microsoft.AspNetCore.Identity;

namespace Otito.Web.Models
{
public class ApplicationUser : IdentityUser
    {
    }

    //public class ClaimsPrincipal : IPrincipal
    //{
    //    public IIdentity Identity { get; }
    //    public IEnumerable<ClaimsIdentity> Identities { get; }
    //    public IEnumerable<Claim> Claims { get; }

    //    public bool IsInRole(string role) { /*...*/ return false; }
    //    //public Claim FindFirst(string type) { /*...*/ }
    //    //public Claim HasClaim(string type, string value) { /*...*/ }
    //}


}
