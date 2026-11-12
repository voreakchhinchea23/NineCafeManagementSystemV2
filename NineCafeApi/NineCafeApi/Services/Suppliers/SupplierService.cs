using Microsoft.EntityFrameworkCore;
using NineCafeApi.Data;
using NineCafeApi.DTOs.Suppliers;
using NineCafeApi.Models;

namespace NineCafeApi.Services.Suppliers
{
    public class SupplierService : ISupplierService
    {
        private readonly AppDbContext _context;

        public SupplierService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SupplierResponseDto>> GetAllSuppliersAsync()
        {
            return await _context.Suppliers
                .OrderBy(s => s.Name)
                .Select(s => new SupplierResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    ContactInfo = s.ContactInfo,
                    Address = s.Address,
                    Product = s.Product,
                    Note = s.Note,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<SupplierResponseDto?> GetSupplierByIdAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return null;

            return new SupplierResponseDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactInfo = supplier.ContactInfo,
                Address = supplier.Address,
                Product = supplier.Product,
                Note = supplier.Note,
                CreatedAt = supplier.CreatedAt,
                UpdatedAt = supplier.UpdatedAt
            };
        }

        public async Task<SupplierResponseDto> CreateSupplierAsync(SupplierCreateDto dto)
        {
            var now = DateTime.UtcNow;

            var supplier = new Supplier
            {
                Name = dto.Name,
                ContactInfo = dto.ContactInfo,
                Address = dto.Address,
                Product = dto.Product,
                Note = dto.Note,
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return new SupplierResponseDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactInfo = supplier.ContactInfo,
                Address = supplier.Address,
                Product = supplier.Product,
                Note = supplier.Note,
                CreatedAt = supplier.CreatedAt,
                UpdatedAt = supplier.UpdatedAt
            };
        }

        public async Task<SupplierResponseDto?> UpdateSupplierAsync(int id, SupplierCreateDto dto)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return null;

            supplier.Name = dto.Name;
            supplier.ContactInfo = dto.ContactInfo;
            supplier.Address = dto.Address;
            supplier.Product = dto.Product;
            supplier.Note = dto.Note;
            supplier.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new SupplierResponseDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactInfo = supplier.ContactInfo,
                Address = supplier.Address,
                Product = supplier.Product,
                Note = supplier.Note,
                CreatedAt = supplier.CreatedAt,
                UpdatedAt = supplier.UpdatedAt
            };
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return false;

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
