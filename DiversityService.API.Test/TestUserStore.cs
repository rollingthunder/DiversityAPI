namespace DiversityService.API.Test
{
    using DiversityService.API.WebHost.Models;
    using Microsoft.AspNet.Identity;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class TestUserStore : IUserStore<ApplicationUser>
    {
        private HashSet<ApplicationUser> Users = new HashSet<ApplicationUser>();

        public async Task CreateAsync(ApplicationUser user)
        {
            Users.Add(user);
        }

        public async Task DeleteAsync(ApplicationUser user)
        {
            Users.Remove(user);
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return FindByNameAsync(userId);
        }

        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return Users.SingleOrDefault(x => x.UserName == userName);
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
        }

        public void Dispose()
        {
        }
    }
}