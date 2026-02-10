using Microsoft.AspNetCore.Mvc;
using Procurement.Api.Services;
using Procurement.Application.Contracts;
using Procurement.Application.Services;

namespace Procurement.Api.Controllers;

[ApiController]
[Route("api/suppliers")]
public class SuppliersController : ControllerBase
{
    private readonly ProcurementService _svc;
    private readonly QueryService _query;

    public SuppliersController(ProcurementService svc, QueryService query)
    {
        _svc = svc ?? throw new ArgumentNullException(nameof(svc));
        _query = query ?? throw new ArgumentNullException(nameof(query));
    }

    [HttpPost]
    public async Task<IActionResult> Register(
        [FromBody] RegisterSupplierCommand cmd,
        CancellationToken ct)
    {
        var id = await _svc.RegisterSupplier(cmd, ct);
        return CreatedAtAction(nameof(Register), new { id }, new { id });
    }

    [HttpPost("{id:guid}/qualify")]
    public async Task<IActionResult> Qualify(
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        await _svc.QualifySupplier(new QualifySupplierCommand(id), ct);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20,
        CancellationToken ct = default)
    {
        var safeTake = Math.Clamp(take, 1, 100);
        var result = await _query.ListSuppliers(skip, safeTake, ct);
        return Ok(result);
    }
}
