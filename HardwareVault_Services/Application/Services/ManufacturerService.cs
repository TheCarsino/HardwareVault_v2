// ============================================================
// APPLICATION LAYER — MANUFACTURER SERVICE
// Handles manufacturer queries for filter dropdowns and admin UI
// ============================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HardwareVault_Services.Application.DTOs;
using HardwareVault_Services.Application.Interfaces;
using HardwareVault_Services.Domain.Interfaces;

namespace HardwareVault_Services.Application.Services
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly IUnitOfWork _uow;

        public ManufacturerService(IUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public async Task<List<ManufacturerDto>> GetAllAsync()
        {
            var manufacturers = await _uow.Manufacturers.GetAllWithCountsAsync();

            return manufacturers.Select(m => new ManufacturerDto
            {
                ManufacturerId = m.Id,
                Name           = m.Name,
                Type           = m.ProductType,
                Website        = m.Website,
                CpuCount       = m.Cpus.Count,
                GpuCount       = m.Gpus.Count,
                DeviceCount    = m.Cpus.Sum(c => c.Devices.Count)
                               + m.Gpus.Sum(g => g.Devices.Count)
            }).ToList();
        }
    }
}
