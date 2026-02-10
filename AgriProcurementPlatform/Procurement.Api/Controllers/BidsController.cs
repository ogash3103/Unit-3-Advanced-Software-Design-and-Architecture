using Microsoft.AspNetCore.Mvc;
using Procurement.Api.Services;
using Procurement.Application.Contracts;
using Procurement.Application.Services;

namespace Procurement.Api.Controllers;

[ApiController]
[Route("api/bids")]
public class BidsController : ControllerBase
{
    private readonly ProcurementService _svc;
    private readonly QueryService _query;

    public BidsController(ProcurementService svc, QueryService query)
    {
        _svc = svc ?? throw new ArgumentNullException(nameof(svc));
        _query = query ?? throw new ArgumentNullException(nameof(query));
    }

    [HttpPost]
    public async Task<IActionResult> Submit(
        [FromBody] SubmitBidCommand cmd,
        CancellationToken ct)
    {
        var id = await _svc.SubmitBid(cmd, ct);
        return CreatedAtAction(nameof(Submit), new { id }, new { id });
    }

    [HttpGet("by-opportunity/{opportunityId:guid}")]
    public async Task<IActionResult> ListByOpportunity(
        [FromRoute] Guid opportunityId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20,
        CancellationToken ct = default)
    {
        var safeTake = Math.Clamp(take, 1, 100);
        var result = await _query.ListBidsByOpportunity(opportunityId, skip, safeTake, ct);
        return Ok(result);
    }
}
