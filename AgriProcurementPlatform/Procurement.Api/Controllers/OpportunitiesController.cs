using Microsoft.AspNetCore.Mvc;
using Procurement.Application.Contracts;
using Procurement.Application.Services;
using Procurement.Api.Services;

namespace Procurement.Api.Controllers;

[ApiController]
[Route("api/opportunities")]
public class OpportunitiesController : ControllerBase
{
    private readonly ProcurementService _svc;
    private readonly QueryService _query;

    public OpportunitiesController(ProcurementService svc, QueryService query)
    {
        _svc = svc;
        _query = query;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOpportunityCommand cmd, CancellationToken ct)
    {
        var id = await _svc.CreateOpportunity(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20, CancellationToken ct = default)
        => Ok(await _query.ListOpportunities(skip, Math.Clamp(take, 1, 100), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var item = await _query.GetOpportunity(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost("{id:guid}/close")]
public async Task<IActionResult> Close([FromRoute] Guid id, CancellationToken ct)
{
    await _svc.CloseOpportunity(new CloseOpportunityCommand(id), ct);
    return NoContent();
}

}
