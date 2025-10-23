using EventLink_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLink_Repositories.Interface
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> IsValidRoleAsync(string role);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<User?> GetActiveUserByIdAsync(Guid id);
        Task<User?> GetActiveUserByEmailAsync(string email);
        Task<List<User>> GetPartnersByPartnershipAsync(Guid partnershipId)
    }
}
