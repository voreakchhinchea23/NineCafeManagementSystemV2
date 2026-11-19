using NineCafeApi.DTOs.Suppliers;

namespace NineCafeApi.Services.Suppliers
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierResponseDto>> GetAllSuppliersAsync();
        Task<SupplierResponseDto?> GetSupplierByIdAsync(int id);
        Task<SupplierResponseDto> CreateSupplierAsync(SupplierCreateDto dto);
        Task<SupplierResponseDto?> UpdateSupplierAsync(int id, SupplierCreateDto dto);
        Task<bool> DeleteSupplierAsync(int id);
    }
}
