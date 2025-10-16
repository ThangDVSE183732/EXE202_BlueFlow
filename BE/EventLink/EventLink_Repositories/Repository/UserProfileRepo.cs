using EventLink_Repositories.DBContext;
using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLink_Repositories.Repository
{
    public class UserProfileRepo : GenericRepository<UserProfile>, IUserProfileRepo
    {
        private readonly EventLinkDBContext _context;
        public UserProfileRepo(EventLinkDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<UserProfile>> GetAllUserProfilesAsync()
        {
            return await _context.UserProfiles.Include(up => up.User).ToListAsync();
        }

        public async Task<UserProfile> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserProfiles.Include(up => up.User).FirstOrDefaultAsync(u => u.Id == userId) ?? new UserProfile();
        }

        public async Task<bool> UserProfileExistsAsync(Guid userId)
        {
            return await _context.UserProfiles.AnyAsync(up => up.Id == userId);
        }
    }
}
