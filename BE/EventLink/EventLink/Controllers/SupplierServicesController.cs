using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Eventlink_Services.Interface;
using Eventlink_Services.Response;
using static Eventlink_Services.Request.SupplierServiceRequest;
using Microsoft.AspNetCore.Authorization;

namespace EventLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SupplierServicesController : ControllerBase
    {
        private readonly ISupplierServiceService _supplierServiceService;

        public SupplierServicesController(ISupplierServiceService supplierServiceService)
        {
            _supplierServiceService = supplierServiceService;
        }

        // GET: api/SupplierServices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierServiceResponse>>> GetSupplierServices(string? category, decimal? minPrice, decimal? maxPrice)
        {
            var supplierServices = await _supplierServiceService.GetSupplierServicesAsync(category, minPrice, maxPrice);

            var result = supplierServices.Select(s => new SupplierServiceResponse
            {
                Id = s.Id,
                SupplierId = s.SupplierId,
                ServiceName = s.ServiceName,
                ServiceCategory = s.ServiceCategory,
                Description = s.Description,
                MinPrice = s.MinPrice,
                MaxPrice = s.MaxPrice,
                IsActive = s.IsActive,
                BasePrice = s.BasePrice,
                PriceUnit = s.PriceUnit,
                ServiceImages = s.ServiceImages,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList();

            return Ok(new
            {
                success = true,
                message = "Supplier services retrieved successfully",
                data = result,
                count = result.Count
            });
        }

        // GET: api/SupplierServices/5
        [HttpGet("getById/{id}")]
        public async Task<ActionResult<SupplierServiceResponse>> GetSupplierService(Guid id)
        {
            var supplierService = await _supplierServiceService.GetSupplierServicesByIdAsync(id);

            var result = new SupplierServiceResponse
            {
                Id = supplierService.Id,
                SupplierId = supplierService.SupplierId,
                ServiceName = supplierService.ServiceName,
                ServiceCategory = supplierService.ServiceCategory,
                Description = supplierService.Description,
                MinPrice = supplierService.MinPrice,
                MaxPrice = supplierService.MaxPrice,
                IsActive = supplierService.IsActive,
                BasePrice = supplierService.BasePrice,
                PriceUnit = supplierService.PriceUnit,
                ServiceImages = supplierService.ServiceImages,
                CreatedAt = supplierService.CreatedAt,
                UpdatedAt = supplierService.UpdatedAt
            };

            return Ok(new
            {
                success = true,
                message = "Supplier service retrieved successfully",
                data = result
            });
        }

        [HttpGet("getByCategories/{categories}")]
        public async Task<ActionResult<IEnumerable<SupplierServiceResponse>>> GetSupplierServicesByCategories(string categories)
        {
            var supplierServices = await _supplierServiceService.GetSupplierServicesByCategoriesAsync(categories);

            var result = supplierServices.Select(s => new SupplierServiceResponse
            {
                Id = s.Id,
                SupplierId = s.SupplierId,
                ServiceName = s.ServiceName,
                ServiceCategory = s.ServiceCategory,
                Description = s.Description,
                MinPrice = s.MinPrice,
                MaxPrice = s.MaxPrice,
                IsActive = s.IsActive,
                BasePrice = s.BasePrice,
                PriceUnit = s.PriceUnit,
                ServiceImages = s.ServiceImages,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList();

            return Ok(new
            {
                success = true,
                message = "Supplier services retrieved successfully",
                data = result,
                count = result.Count
            });
        }

        // PUT: api/SupplierServices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<SupplierServiceResponse>> PutSupplierService(Guid id, [FromBody] UpdateSupplierService request)
        {
            try
            {
                var existingSupplierService = await _supplierServiceService.GetSupplierServicesByIdAsync(id);
                if (existingSupplierService == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Supplier service not found"
                    });
                }

                await _supplierServiceService.Update(id, request);

                var result = new SupplierServiceResponse
                {
                    Id = existingSupplierService.Id,
                    SupplierId = existingSupplierService.SupplierId,
                    ServiceName = request.ServiceName,
                    ServiceCategory = request.ServiceCategory,
                    Description = request.Description,
                    MinPrice = request.MinPrice,
                    MaxPrice = request.MaxPrice,
                    IsActive = true,
                    BasePrice = request.BasePrice,
                    PriceUnit = request.PriceUnit,
                    ServiceImages = request.ServiceImages,
                    CreatedAt = existingSupplierService.CreatedAt,
                    UpdatedAt = DateTime.UtcNow
                };

                return Ok(new
                {
                    success = true,
                    message = "Supplier service updated successfully",
                    data = result
                });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while updating the supplier service",
                    error = ex.Message
                });
            }
        }

        // POST: api/SupplierServices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SupplierServiceResponse>> PostSupplierService([FromBody] CreateSupplierService request)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized();

                await _supplierServiceService.CreateAsync(userId, request);

                var result = new SupplierServiceResponse
                {
                    SupplierId = userId,
                    ServiceName = request.ServiceName,
                    ServiceCategory = request.ServiceCategory,
                    Description = request.Description,
                    MinPrice = request.MinPrice,
                    MaxPrice = request.MaxPrice,
                    IsActive = true,
                    BasePrice = request.BasePrice,
                    PriceUnit = request.PriceUnit,
                    ServiceImages = request.ServiceImages
                };

                return Ok(new
                {
                    success = true,
                    message = "Supplier service created successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while creating the supplier service",
                    error = ex.Message
                });
            }
        }

        // DELETE: api/SupplierServices/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SupplierServiceResponse>> DeleteSupplierService(Guid id)
        {
            try
            {
                var existingSupplierService = await _supplierServiceService.GetSupplierServicesByIdAsync(id);
                if (existingSupplierService == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Supplier service not found"
                    });
                }

                await _supplierServiceService.Remove(id);

                var result = new SupplierServiceResponse
                {
                    Id = existingSupplierService.Id,
                    SupplierId = existingSupplierService.SupplierId,
                    ServiceName = existingSupplierService.ServiceName,
                    ServiceCategory = existingSupplierService.ServiceCategory,
                    Description = existingSupplierService.Description,
                    MinPrice = existingSupplierService.MinPrice,
                    MaxPrice = existingSupplierService.MaxPrice,
                    IsActive = existingSupplierService.IsActive,
                    BasePrice = existingSupplierService.BasePrice,
                    PriceUnit = existingSupplierService.PriceUnit,
                    ServiceImages = existingSupplierService.ServiceImages,
                    CreatedAt = existingSupplierService.CreatedAt,
                    UpdatedAt = existingSupplierService.UpdatedAt
                };
                return Ok(new
                {
                    success = true,
                    message = "Supplier service deleted successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while deleting the supplier service",
                    error = ex.Message
                });
            }
        }

        private bool SupplierServiceExists(Guid id)
        {
            return _supplierServiceService.GetSupplierServicesByIdAsync(id) != null;
        }
    }
}
