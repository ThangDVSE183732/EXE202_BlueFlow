using EventLink_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLink_Repositories.Interface
{
    public interface IBrandProfileRepository : IGenericRepository<BrandProfile>
    {
        Task<BrandProfile> GetByUserIdAsync(Guid userId);
    }
}
