using NineCafeApi.DTOs.Suppliers;

namespace NineCafeApi.Services.Suppliers
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierResponseDto>> GetAllAsync();
        Task<SupplierResponseDto?> GetByIdAsync(int id);
        Task<SupplierResponseDto> CreateAsync(SupplierCreateDto dto);
        Task<SupplierResponseDto?> UpdateAsync(int id, SupplierCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
