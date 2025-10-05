using EventLink_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Interface
{
    public interface IUserService
    {
        Task<List<User>> GetSuppliersAsync(string? category, string? location);
        Task<List<User>> GetOrganizersAsync(decimal? budget, string? targetAudience, string? packageType);
    }
}
