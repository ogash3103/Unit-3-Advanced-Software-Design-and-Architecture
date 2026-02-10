using Microsoft.EntityFrameworkCore;
using Procurement.Infrastructure.Persistence;
using Procurement.Api.Contracts;

namespace Procurement.Api.Services;

public sealed class QueryService
{
    private readonly ProcurementDbContext _db;
    public QueryService(ProcurementDbContext db) => _db = db;

    public async Task<IReadOnlyList<OpportunityResponse>> ListOpportunities(int skip, int take, CancellationToken ct)
        => await _db.OpportunitiesSet
            .OrderByDescending(x => x.DeadlineUtc)
            .Skip(skip).Take(take)
            .Select(x => new OpportunityResponse(
                x.Id, x.Title, x.ProductCategory, x.Quantity, x.DeadlineUtc, x.RegionCode, x.Status.ToString()))
            .ToListAsync(ct);

    public async Task<OpportunityResponse?> GetOpportunity(Guid id, CancellationToken ct)
        => await _db.OpportunitiesSet
            .Where(x => x.Id == id)
            .Select(x => new OpportunityResponse(
                x.Id, x.Title, x.ProductCategory, x.Quantity, x.DeadlineUtc, x.RegionCode, x.Status.ToString()))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<SupplierResponse>> ListSuppliers(int skip, int take, CancellationToken ct)
        => await _db.SuppliersSet
            .OrderBy(x => x.LegalName)
            .Skip(skip).Take(take)
            .Select(x => new SupplierResponse(x.Id, x.LegalName, x.RegionCode, x.IsQualified))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<BidResponse>> ListBidsByOpportunity(Guid opportunityId, int skip, int take, CancellationToken ct)
        => await _db.BidsSet
            .Where(x => x.OpportunityId == opportunityId)
            .OrderByDescending(x => x.SubmittedAtUtc)
            .Skip(skip).Take(take)
            .Select(x => new BidResponse(x.Id, x.OpportunityId, x.SupplierId, x.UnitPrice, x.SubmittedAtUtc))
            .ToListAsync(ct);
}
