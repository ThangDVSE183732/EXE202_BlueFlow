using EventLink_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLink_Repositories.Interface
{
    public interface IPartnershipRepository : IGenericRepository<Partnership>
    {
        Task<Partnership> GetByIdAsync(Guid id);
        Task UpdateAsync(Partnership partnership);
    }
}
