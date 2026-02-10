using Procurement.Application.Abstractions;
using Procurement.Application.Contracts;
using Procurement.Domain.Bids;
using Procurement.Domain.Opportunities;
using Procurement.Domain.Suppliers;

namespace Procurement.Application.Services;

public sealed class ProcurementService
{
    private readonly IProcurementDbContext _db;

    public ProcurementService(IProcurementDbContext db) => _db = db;

    public async Task<Guid> CreateOpportunity(CreateOpportunityCommand cmd, CancellationToken ct)
    {
        var entity = new Opportunity(cmd.Title, cmd.ProductCategory, cmd.Quantity, cmd.DeadlineUtc, cmd.RegionCode);
        _db.AddOpportunity(entity);
        await _db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<Guid> RegisterSupplier(RegisterSupplierCommand cmd, CancellationToken ct)
    {
        var supplier = new Supplier(cmd.LegalName, cmd.RegionCode);
        _db.AddSupplier(supplier);
        await _db.SaveChangesAsync(ct);
        return supplier.Id;
    }

    public async Task QualifySupplier(QualifySupplierCommand cmd, CancellationToken ct)
    {
        var supplier = await _db.FindSupplier(cmd.SupplierId, ct)
            ?? throw new InvalidOperationException("Supplier not found");

        supplier.Qualify();
        await _db.SaveChangesAsync(ct);
    }

    public async Task<Guid> SubmitBid(SubmitBidCommand cmd, CancellationToken ct)
    {
        var opp = await _db.FindOpportunity(cmd.OpportunityId, ct)
            ?? throw new InvalidOperationException("Opportunity not found");

        if (opp.Status != OpportunityStatus.Open) throw new InvalidOperationException("Opportunity is closed");
        if (opp.DeadlineUtc <= DateTime.UtcNow) throw new InvalidOperationException("Deadline passed");

        var supplier = await _db.FindSupplier(cmd.SupplierId, ct)
            ?? throw new InvalidOperationException("Supplier not found");

        if (!supplier.IsQualified) throw new InvalidOperationException("Supplier is not qualified");

        var bid = new Bid(cmd.OpportunityId, cmd.SupplierId, cmd.UnitPrice);
        _db.AddBid(bid);
        await _db.SaveChangesAsync(ct);
        return bid.Id;
    }

    public async Task CloseOpportunity(CloseOpportunityCommand cmd, CancellationToken ct)
    {
    var opp = await _db.FindOpportunity(cmd.OpportunityId, ct)
        ?? throw new InvalidOperationException("Opportunity not found");

    opp.Close();
    await _db.SaveChangesAsync(ct);
    }

}
