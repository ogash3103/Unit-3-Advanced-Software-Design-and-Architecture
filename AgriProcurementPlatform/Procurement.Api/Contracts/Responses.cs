namespace Procurement.Api.Contracts;

public record OpportunityResponse(
    Guid Id, string Title, string ProductCategory, decimal Quantity,
    DateTime DeadlineUtc, string RegionCode, string Status);

public record SupplierResponse(Guid Id, string LegalName, string RegionCode, bool IsQualified);

public record BidResponse(Guid Id, Guid OpportunityId, Guid SupplierId, decimal UnitPrice, DateTime SubmittedAtUtc);
