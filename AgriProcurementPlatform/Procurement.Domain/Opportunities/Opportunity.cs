using Procurement.Domain.Common;

namespace Procurement.Domain.Opportunities;

public sealed class Opportunity : Entity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; }
    public string ProductCategory { get; private set; }
    public decimal Quantity { get; private set; }
    public DateTime DeadlineUtc { get; private set; }
    public string RegionCode { get; private set; }
    public OpportunityStatus Status { get; private set; } = OpportunityStatus.Open;

    private Opportunity() { } // EF

    public Opportunity(string title, string productCategory, decimal quantity, DateTime deadlineUtc, string regionCode)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title required");
        if (quantity <= 0) throw new ArgumentException("Quantity must be > 0");
        if (deadlineUtc <= DateTime.UtcNow) throw new ArgumentException("Deadline must be in the future");

        Title = title;
        ProductCategory = productCategory;
        Quantity = quantity;
        DeadlineUtc = deadlineUtc;
        RegionCode = regionCode;

        AddDomainEvent(new OpportunityCreatedEvent(Id, Title));
    }

    public void Close()
    {
        if (Status == OpportunityStatus.Closed) return;
        Status = OpportunityStatus.Closed;
        AddDomainEvent(new OpportunityClosedEvent(Id));
    }
}

public enum OpportunityStatus { Open = 1, Closed = 2 }

public record OpportunityCreatedEvent(Guid OpportunityId, string Title) : DomainEvent(DateTime.UtcNow);
public record OpportunityClosedEvent(Guid OpportunityId) : DomainEvent(DateTime.UtcNow);
