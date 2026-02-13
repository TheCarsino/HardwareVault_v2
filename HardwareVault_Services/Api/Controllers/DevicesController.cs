using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HardwareVault_Services.Application.DTOs;
using HardwareVault_Services.Application.Interfaces;

namespace HardwareVault_Services.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _deviceService;
        private readonly IImportService _importService;
        private readonly ILogger<DevicesController> _logger;

        public DevicesController(
            IDeviceService deviceService,
            IImportService importService,
            ILogger<DevicesController> logger)
        {
            _deviceService = deviceService;
            _importService = importService;
            _logger        = logger;
        }

        // -- GET /api/devices --
        // Query params: page, pageSize, cpuManufacturer, gpuManufacturer,
        //               storageType, minRamInGB, search
        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<DeviceDto>), 200)]
        public async Task<ActionResult<PagedResultDto<DeviceDto>>> GetDevices(
            [FromQuery] int     page             = 1,
            [FromQuery] int     pageSize         = 20,
            [FromQuery] string? cpuManufacturer  = null,
            [FromQuery] string? gpuManufacturer  = null,
            [FromQuery] string? storageType      = null,
            [FromQuery] int?    minRamInGB        = null,
            [FromQuery] string? search           = null)
        {
            if (page < 1)
                return BadRequest(new { Error = "page must be at least 1" });
            if (pageSize is < 1 or > 100)
                return BadRequest(new { Error = "pageSize must be between 1 and 100" });

            var result = await _deviceService.GetDevicesAsync(
                page, pageSize,
                cpuManufacturer, gpuManufacturer,
                storageType, minRamInGB, search);

            return Ok(result);
        }

        // -- GET /api/devices/{id} --
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(DeviceDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<DeviceDto>> GetDeviceById(Guid id)
        {
            var device = await _deviceService.GetDeviceByIdAsync(id);
            if (device is null)
                return NotFound(new { Error = $"Device {id} not found" });

            return Ok(device);
        }

        // -- GET /api/devices/statistics --
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(DeviceStatisticsDto), 200)]
        public async Task<ActionResult<DeviceStatisticsDto>> GetStatistics()
        {
            var stats = await _deviceService.GetStatisticsAsync();
            return Ok(stats);
        }

        // -- POST /api/devices ----
        [HttpPost]
        [ProducesResponseType(typeof(DeviceDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<DeviceDto>> CreateDevice(
            [FromBody] CreateDeviceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var device = await _deviceService.CreateDeviceAsync(dto);
                return CreatedAtAction(
                    nameof(GetDeviceById),
                    new { id = device.DeviceId },
                    device);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // -- PUT /api/devices/{id} --
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(DeviceDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<DeviceDto>> UpdateDevice(
            Guid id,
            [FromBody] UpdateDeviceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var device = await _deviceService.UpdateDeviceAsync(id, dto);
                if (device is null)
                    return NotFound(new { Error = $"Device {id} not found" });

                return Ok(device);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // -- DELETE /api/devices/{id} --
        // Soft delete — sets IsDeleted = true, never removes the row
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteDevice(Guid id)
        {
            var deleted = await _deviceService.DeleteDeviceAsync(id);
            if (!deleted)
                return NotFound(new { Error = $"Device {id} not found" });

            return NoContent();
        }

        // -- POST /api/devices/import --
        // 200  — all rows succeeded
        // 207  — partial success (some rows failed — see Errors array)
        // 400  — file missing, wrong type, or too large
        // 500  — unhandled server error
        [HttpPost("import")]
        [ProducesResponseType(typeof(ImportResultDto), 200)]
        [ProducesResponseType(typeof(ImportResultDto), StatusCodes.Status207MultiStatus)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ImportResultDto>> ImportDevices(IFormFile file)
        {
            if (file is null || file.Length == 0)
                return BadRequest(new { Error = "No file uploaded" });

            if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { Error = "Only .xlsx files are accepted" });

            const long maxBytes = 10 * 1024 * 1024; // 10 MB
            if (file.Length > maxBytes)
                return BadRequest(new { Error = "File cannot exceed 10 MB" });

            try
            {
                using var stream = file.OpenReadStream();
                var result = await _importService.ImportDevicesAsync(stream, file.FileName);

                return result.FailureCount > 0
                    ? StatusCode(StatusCodes.Status207MultiStatus, result)
                    : Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Import failed: {File}", file.FileName);
                return StatusCode(500, new { Error = "An error occurred during import" });
            }
        }
    }
}
