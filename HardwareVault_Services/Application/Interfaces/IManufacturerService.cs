using System.Collections.Generic;
using System.Threading.Tasks;
using HardwareVault_Services.Application.DTOs;

namespace HardwareVault_Services.Application.Interfaces
{
    public interface IManufacturerService
    {
        // GET /api/manufacturers — all manufacturers with counts
        Task<List<ManufacturerDto>> GetAllAsync();
    }
}
