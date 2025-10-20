using EventLink_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLink_Repositories.Interface
{
    public interface ISupplierServiceRepository : IGenericRepository<SupplierService>
    {
        Task<List<SupplierService>> GetSupplierServicesAsync(string? category, decimal? minPrice, decimal? maxPrice);
        Task<SupplierService> GetSupplierServicesByIdAsync(Guid id);
        IQueryable<SupplierService> GetSupplierServicesByCategoriesAsync(string categories);
    }
}
