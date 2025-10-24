using EventLink_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Eventlink_Services.Request.SupplierServiceRequest;

namespace Eventlink_Services.Interface
{
    public interface ISupplierServiceService
    {
        Task<List<SupplierService>> GetSupplierServicesAsync(string? category, decimal? minPrice, decimal? maxPrice);
        Task<SupplierService> GetSupplierServicesByIdAsync(Guid id);
        Task<List<SupplierService>> GetSupplierServicesByCategoriesAsync(string categories);
        Task CreateAsync(Guid userId, CreateSupplierService request);
        Task Update(Guid id ,UpdateSupplierService request);
        Task Remove(Guid id);
    }
}
