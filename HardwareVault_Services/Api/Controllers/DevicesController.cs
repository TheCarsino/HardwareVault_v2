using Microsoft.AspNetCore.Mvc;
using HardwareVault_Services.Application.DTOs;
using HardwareVault_Services.Application.Interfaces;

namespace HardwareVault_Services.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _deviceService;
        private readonly ILogger<DevicesController> _logger;

        public DevicesController(IDeviceService deviceService, ILogger<DevicesController> logger)
        {
            _deviceService = deviceService;
            _logger = logger;
        }

        /// <summary>
        /// Get paginated list of devices with optional filters
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<DeviceDto>>> GetDevices(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? cpuManufacturer = null,
            [FromQuery] string? gpuManufacturer = null,
            [FromQuery] int? minRamInGB = null,
            [FromQuery] string? storageType = null)
        {
            try
            {
                var result = await _deviceService.GetDevicesAsync(
                    page, pageSize, cpuManufacturer, gpuManufacturer, minRamInGB, storageType);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving devices");
                return StatusCode(500, "An error occurred while retrieving devices");
            }
        }

        /// <summary>
        /// Get a specific device by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceDto>> GetDevice(Guid id)
        {
            try
            {
                var device = await _deviceService.GetDeviceByIdAsync(id);
                if (device == null)
                {
                    return NotFound($"Device with ID {id} not found");
                }
                return Ok(device);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving device {DeviceId}", id);
                return StatusCode(500, "An error occurred while retrieving the device");
            }
        }

        /// <summary>
        /// Create a new device
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DeviceDto>> CreateDevice([FromBody] CreateDeviceDto createDto)
        {
            try
            {
                var device = await _deviceService.CreateDeviceAsync(createDto);
                return CreatedAtAction(nameof(GetDevice), new { id = device.DeviceId }, device);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating device");
                return StatusCode(500, "An error occurred while creating the device");
            }
        }

        /// <summary>
        /// Update an existing device
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<DeviceDto>> UpdateDevice(Guid id, [FromBody] UpdateDeviceDto updateDto)
        {
            try
            {
                var device = await _deviceService.UpdateDeviceAsync(id, updateDto);
                if (device == null)
                {
                    return NotFound($"Device with ID {id} not found");
                }
                return Ok(device);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating device {DeviceId}", id);
                return StatusCode(500, "An error occurred while updating the device");
            }
        }

        /// <summary>
        /// Delete a device (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(Guid id)
        {
            try
            {
                var result = await _deviceService.DeleteDeviceAsync(id);
                if (!result)
                {
                    return NotFound($"Device with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting device {DeviceId}", id);
                return StatusCode(500, "An error occurred while deleting the device");
            }
        }

        /// <summary>
        /// Get devices by CPU manufacturer
        /// </summary>
        [HttpGet("by-cpu-manufacturer/{manufacturerName}")]
        public async Task<ActionResult<IEnumerable<DeviceDto>>> GetDevicesByCpuManufacturer(string manufacturerName)
        {
            try
            {
                var devices = await _deviceService.GetDevicesByCpuManufacturerAsync(manufacturerName);
                return Ok(devices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving devices for manufacturer {Manufacturer}", manufacturerName);
                return StatusCode(500, "An error occurred while retrieving devices");
            }
        }
    }
}
