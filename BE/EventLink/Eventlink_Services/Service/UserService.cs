using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using EventLink_Repositories.Repository;
using Eventlink_Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<List<User>> GetOrganizersAsync(decimal? budget, string? targetAudience, string? packageType)
        {
            var query = await _userRepository.GetAllAsync();

            query = query.Where(u => u.Role == "Organizer");

            if (budget.HasValue)
            {
                query = query.Where(u => u.Events.Any(op => op.TotalBudget <= budget.Value));
            }
            if (!string.IsNullOrEmpty(targetAudience))
            {
                query = query.Where(u => u.Events.Any(op => op.TargetAudience.Contains(targetAudience)));
            }
            if (!string.IsNullOrEmpty(packageType))
            {
                query = query.Where(u => u.Events.Any(op => op.SponsorshipNeeds.Contains(packageType)));
            }
            return query.ToList();
        }

        public async Task<List<User>> GetSuppliersAsync(string? category, string? location)
        {
            var query = await _userRepository.GetAllAsync();

            query = query.Where(u => u.Role == "Supplier");

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(u => u.Events.Any(s => s.Category.Contains(category)));
            }
            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(u => u.Events.Any(s => s.Location.Contains(location)));
            }
            //if (price.HasValue)
            //{
            //    query = query.Where(u => u.Events.Any(s => s.Price <= price.Value));
            //}
            return query.ToList();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            return user;
        }
    }
}
