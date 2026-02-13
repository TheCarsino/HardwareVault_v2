using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HardwareVault_Services.Application.DTOs;
using HardwareVault_Services.Application.Interfaces;

namespace HardwareVault_Services.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ManufacturersController : ControllerBase
    {
        private readonly IManufacturerService _manufacturerService;

        public ManufacturersController(IManufacturerService manufacturerService)
        {
            _manufacturerService = manufacturerService;
        }

        // -- GET /api/manufacturers --
        // Used to populate CPU manufacturer and GPU manufacturer filter dropdowns
        [HttpGet]
        [ProducesResponseType(typeof(System.Collections.Generic.List<ManufacturerDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _manufacturerService.GetAllAsync();
            return Ok(result);
        }
    }
}
