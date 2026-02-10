using Procurement.Domain.Common;

namespace Procurement.Domain.Bids;

public sealed class Bid : Entity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid OpportunityId { get; private set; }
    public Guid SupplierId { get; private set; }
    public decimal UnitPrice { get; private set; }
    public DateTime SubmittedAtUtc { get; private set; } = DateTime.UtcNow;

    private Bid() { }

    public Bid(Guid opportunityId, Guid supplierId, decimal unitPrice)
    {
        if (opportunityId == Guid.Empty) throw new ArgumentException("OpportunityId required");
        if (supplierId == Guid.Empty) throw new ArgumentException("SupplierId required");
        if (unitPrice <= 0) throw new ArgumentException("UnitPrice must be > 0");

        OpportunityId = opportunityId;
        SupplierId = supplierId;
        UnitPrice = unitPrice;

        AddDomainEvent(new BidSubmittedEvent(Id, OpportunityId, SupplierId, UnitPrice));
    }
}

public record BidSubmittedEvent(Guid BidId, Guid OpportunityId, Guid SupplierId, decimal UnitPrice)
    : DomainEvent(DateTime.UtcNow);

