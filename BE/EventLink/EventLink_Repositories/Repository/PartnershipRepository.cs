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
    public class PartnershipRepository : GenericRepository<Partnership>, IPartnershipRepository
    {
        private readonly EventLinkDBContext _context;
        public PartnershipRepository(EventLinkDBContext context) : base(context)
        {
        }

        public async Task UpdateAsync(Partnership partnership)
        {
            _context.Update(partnership);
        }

        public async Task<Partnership> GetByIdAsync(Guid id)
        {
            return await _context.Partnerships.FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
