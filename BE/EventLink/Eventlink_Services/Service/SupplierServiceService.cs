using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Eventlink_Services.Request.SupplierServiceRequest;

namespace Eventlink_Services.Service
{
    public class SupplierServiceService : ISupplierServiceService
    {
        private readonly List<string> _validCategories = new() { "Catering", "Photography", "Venue", "Entertainment", "Decoration", "Transportation", "Security", "Audio/Visual", "Marketing", "Other" };
        private readonly ISupplierServiceRepository _supplierServiceRepository;
        public SupplierServiceService(ISupplierServiceRepository supplierServiceRepository)
        {
            _supplierServiceRepository = supplierServiceRepository;
        }
        public async Task CreateAsync(Guid userId, CreateSupplierService request)
        {
            try
            {
                // Tách category từ request, loại bỏ trùng & khoảng trắng
                var requestedCategories = request.ServiceCategory?
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList() ?? new List<string>();

                // Kiểm tra hợp lệ
                var invalidCategories = requestedCategories
                    .Where(c => !_validCategories.Contains(c, StringComparer.OrdinalIgnoreCase))
                    .ToList();

                if (invalidCategories.Any())
                {
                    throw new ArgumentException($"Invalid categories: {string.Join(", ", invalidCategories)}");
                }

                // Nối lại thành chuỗi lưu vào DB
                var combinedCategories = string.Join(",", requestedCategories);

                var supplierService = new SupplierService
                {
                    Id = Guid.NewGuid(),
                    SupplierId = userId,
                    ServiceName = request.ServiceName,
                    ServiceCategory = combinedCategories,
                    Description = request.Description,
                    BasePrice = request.BasePrice,
                    PriceUnit = request.PriceUnit,
                    MinPrice = request.MinPrice,
                    MaxPrice = request.MaxPrice,
                    ServiceImages = request.ServiceImages,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _supplierServiceRepository.AddAsync(supplierService);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating supplier service: {ex.Message}");
            }
        }

        public async Task<List<SupplierService>> GetSupplierServicesAsync(string? category, decimal? minPrice, decimal? maxPrice)
        {
            return await _supplierServiceRepository.GetSupplierServicesAsync(category, minPrice, maxPrice);
        }

        public async Task Remove(Guid id)
        {
            try
            {
                var existingService = await _supplierServiceRepository.GetSupplierServicesByIdAsync(id);
                if (existingService == null)
                {
                    throw new Exception("Supplier service not found");
                }
                _supplierServiceRepository.Remove(existingService);
            }
            catch (Exception)
            {
            }
        }

        public async Task Update(Guid id, UpdateSupplierService request)
        {
            try
            {
                // Tách category từ request, loại bỏ trùng & khoảng trắng
                var requestedCategories = request.ServiceCategory?
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList() ?? new List<string>();

                // Kiểm tra hợp lệ
                var invalidCategories = requestedCategories
                    .Where(c => !_validCategories.Contains(c, StringComparer.OrdinalIgnoreCase))
                    .ToList();

                if (invalidCategories.Any())
                {
                    throw new ArgumentException($"Invalid categories: {string.Join(", ", invalidCategories)}");
                }

                // Nối lại thành chuỗi lưu vào DB
                var combinedCategories = string.Join(",", requestedCategories);

                var existingService = await _supplierServiceRepository.GetSupplierServicesByIdAsync(id);
                if (existingService == null)
                {
                    throw new Exception("Supplier service not found");
                }
                existingService.ServiceName = request.ServiceName;
                existingService.ServiceCategory = combinedCategories;
                existingService.Description = request.Description;
                existingService.BasePrice = request.BasePrice;
                existingService.PriceUnit = request.PriceUnit;
                existingService.MinPrice = request.MinPrice;
                existingService.MaxPrice = request.MaxPrice;
                existingService.ServiceImages = request.ServiceImages;
                existingService.UpdatedAt = DateTime.UtcNow;
                _supplierServiceRepository.Update(existingService);
            }
            catch (Exception)
            {
            }
        }

        public async Task<SupplierService> GetSupplierServicesByIdAsync(Guid id)
        {
            return await _supplierServiceRepository.GetSupplierServicesByIdAsync(id) ?? new SupplierService();
        }

        public async Task<List<SupplierService>> GetSupplierServicesByCategoriesAsync(string categories)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(categories))
                    throw new ArgumentException("Categories cannot be empty.");

                // Tách danh sách category từ input
                var categoryList = categories
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (!categoryList.Any())
                    throw new ArgumentException("No valid categories provided.");

                // Lấy query từ repository (giả sử có hàm Query())
                var query = _supplierServiceRepository.GetSupplierServicesByCategoriesAsync(categories);

                // Tạo điều kiện OR cho các category
                // Mỗi category phải match chính xác trong chuỗi ServiceCategory (dùng Contains thông minh)
                var services = await query
                    .Where(s => categoryList.Any(cat =>
                        ("," + s.ServiceCategory + ",").Contains("," + cat + ",")
                    ))
                    .ToListAsync();

                return services;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving supplier services by categories: {ex.Message}");
            }
        }

    }
}
