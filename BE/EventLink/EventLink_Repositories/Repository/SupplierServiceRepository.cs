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
    public class SupplierServiceRepository : GenericRepository<SupplierService>, ISupplierServiceRepository
    {
        private readonly EventLinkDBContext _context;
        public SupplierServiceRepository(EventLinkDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<SupplierService>> GetSupplierServicesAsync(string? category, decimal? minPrice, decimal? maxPrice)
        {
            return await _context.SupplierServices
                .Where(s => (string.IsNullOrEmpty(category) || s.ServiceCategory.Contains(category)) &&
                            (!minPrice.HasValue || s.MinPrice >= minPrice.Value) &&
                            (!maxPrice.HasValue || s.MaxPrice <= maxPrice.Value))
                .ToListAsync();
        }

        public IQueryable<SupplierService> GetSupplierServicesByCategoriesAsync(string categories)
        {
            return _context.SupplierServices
                .Where(s => s.ServiceCategory.Contains(categories))
                .AsQueryable();
        }

        public async Task<SupplierService> GetSupplierServicesByIdAsync(Guid id)
        {
            return await _context.SupplierServices.FirstOrDefaultAsync(s => s.Id.Equals(id));
        }
    }
}
