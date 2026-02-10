using Procurement.Domain.Bids;
using Procurement.Domain.Opportunities;
using Procurement.Domain.Suppliers;

namespace Procurement.Application.Abstractions;

public interface IProcurementDbContext
{
    IQueryable<Opportunity> Opportunities { get; }
    IQueryable<Supplier> Suppliers { get; }
    IQueryable<Bid> Bids { get; }

    void AddOpportunity(Opportunity opportunity);
    void AddSupplier(Supplier supplier);
    void AddBid(Bid bid);

    Task<Supplier?> FindSupplier(Guid id, CancellationToken ct);
    Task<Opportunity?> FindOpportunity(Guid id, CancellationToken ct);

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
