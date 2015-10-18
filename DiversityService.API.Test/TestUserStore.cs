namespace DiversityService.API.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DiversityService.API.WebHost.Models;
    using Microsoft.AspNet.Identity;

    public class TestUserStore : IUserStore<ApplicationUser>
    {
        public readonly HashSet<ApplicationUser> Users;

        public TestUserStore(IEnumerable<ApplicationUser> users = null)
        {
            Users = new HashSet<ApplicationUser>(users ?? Enumerable.Empty<ApplicationUser>());
        }

        public Task CreateAsync(ApplicationUser user)
        {
            return Task.FromResult(Users.Add(user));
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            return Task.FromResult(Users.Remove(user));
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return FindByNameAsync(userId);
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return Task.FromResult(Users.SingleOrDefault(x => x.UserName == userName));
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            Users.Remove(user);
            Users.Add(user);
        }

        public void Dispose()
        {
        }
    }
}