namespace DiversityService.API.WebHost.Models
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public static class IdentityUserClaimExtensions
    {
        public static IdentityUserClaim AddClaim(this IdentityUser @this, Claim claim)
        {
            Contract.Requires<ArgumentNullException>(@this != null, "This");
            Contract.Requires<ArgumentNullException>(claim != null, "claim");

            var userClaim = new IdentityUserClaim();

            userClaim.UserId = @this.Id;
            userClaim.ClaimType = claim.Type;
            userClaim.ClaimValue = claim.Value;

            @this.Claims.Add(userClaim);

            return userClaim;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("UserDB", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    // You can add profile data for the user by adding more properties to your ApplicationUser
    // class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType 
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);

            // Add custom user claims here 
            var identityClaims = from c in Claims
                                 select new Claim(c.ClaimType, c.ClaimValue);

            userIdentity.AddClaims(identityClaims);

            return userIdentity;
        }
    }
}