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
    public class BrandProfileRepository : GenericRepository<BrandProfile>, IBrandProfileRepository
    {
        private readonly EventLinkDBContext _context;
        public BrandProfileRepository(EventLinkDBContext context) : base(context)
        {
            _context = context;
        }
        public async Task<BrandProfile> GetByUserIdAsync(Guid userId)
        {
            return await _context.BrandProfiles.FirstOrDefaultAsync(b => b.UserId == userId);
        }
    }
}
