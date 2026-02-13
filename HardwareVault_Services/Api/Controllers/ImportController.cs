using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HardwareVault_Services.Application.DTOs;
using HardwareVault_Services.Application.Interfaces;

namespace HardwareVault_Services.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ImportController : ControllerBase
    {
        private readonly IImportService _importService;
        private readonly ILogger<ImportController> _logger;

        public ImportController(
            IImportService importService,
            ILogger<ImportController> logger)
        {
            _importService = importService;
            _logger        = logger;
        }

        // -- GET /api/import/history?page=1&pageSize=20 --
        [HttpGet("history")]
        [ProducesResponseType(typeof(PagedResultDto<ImportJobDto>), 200)]
        public async Task<ActionResult<PagedResultDto<ImportJobDto>>> GetHistory(
            [FromQuery] int page     = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page < 1) return BadRequest(new { Error = "page must be at least 1" });
            if (pageSize is < 1 or > 100)
                return BadRequest(new { Error = "pageSize must be between 1 and 100" });

            var result = await _importService.GetImportHistoryAsync(page, pageSize);
            return Ok(result);
        }

        // -- GET /api/import/recent?limit=5 --
        // Used by the dashboard "recent imports" widget
        [HttpGet("recent")]
        [ProducesResponseType(typeof(System.Collections.Generic.List<ImportJobDto>), 200)]
        public async Task<IActionResult> GetRecent([FromQuery] int limit = 5)
        {
            if (limit is < 1 or > 50)
                return BadRequest(new { Error = "limit must be between 1 and 50" });

            var result = await _importService.GetRecentImportsAsync(limit);
            return Ok(result);
        }

        // -- GET /api/import/{jobId} --
        [HttpGet("{jobId:guid}")]
        [ProducesResponseType(typeof(ImportJobDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ImportJobDto>> GetJob(Guid jobId)
        {
            var job = await _importService.GetImportJobAsync(jobId);
            if (job is null)
                return NotFound(new { Error = $"Import job {jobId} not found" });

            return Ok(job);
        }
    }
}
