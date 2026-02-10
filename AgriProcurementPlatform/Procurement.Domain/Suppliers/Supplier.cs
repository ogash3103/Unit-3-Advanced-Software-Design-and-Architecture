using Procurement.Domain.Common;

namespace Procurement.Domain.Suppliers;

public sealed class Supplier : Entity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string LegalName { get; private set; }
    public string RegionCode { get; private set; }
    public bool IsQualified { get; private set; }

    private Supplier() { }

    public Supplier(string legalName, string regionCode)
    {
        if (string.IsNullOrWhiteSpace(legalName)) throw new ArgumentException("Legal name required");
        LegalName = legalName;
        RegionCode = regionCode;
        IsQualified = false;

        AddDomainEvent(new SupplierRegisteredEvent(Id, LegalName));
    }

    public void Qualify()
    {
        if (IsQualified) return;
        IsQualified = true;
        AddDomainEvent(new SupplierQualifiedEvent(Id));
    }
}

public record SupplierRegisteredEvent(Guid SupplierId, string LegalName) : DomainEvent(DateTime.UtcNow);
public record SupplierQualifiedEvent(Guid SupplierId) : DomainEvent(DateTime.UtcNow);
