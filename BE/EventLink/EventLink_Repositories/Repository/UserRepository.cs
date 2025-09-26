using EventLink_Repositories.DBContext;
using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using EventLink_Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace EventLink_Repositories.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly List<string> _validRoles = new() { "Organizer", "Supplier", "Sponsor" };

        public UserRepository(EventLinkDBContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetActiveUserByEmailAsync(string email)
        {
            return await FirstOrDefaultAsync(u =>
                u.Email.ToLower() == email.ToLower() &&
                u.IsActive == true);
        }

        public async Task<User?> GetActiveUserByIdAsync(Guid id)
        {
            return await FirstOrDefaultAsync(u =>
                u.Id == id &&
                u.IsActive == true);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await ExistsAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public Task<bool> IsValidRoleAsync(string role)
        {
            return Task.FromResult(_validRoles.Contains(role));
        }

        public async Task<User> CreateUserAsync(User user)
        {
            user.Id = Guid.NewGuid();
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            user.IsActive = true;  // Set as true (not nullable)
            user.EmailVerified = false;  // Set as false (not nullable)

            await AddAsync(user);
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            Update(user);
            return user;
        }
    }
}