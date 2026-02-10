namespace Procurement.Application.Contracts;

public record CreateOpportunityCommand(string Title, string ProductCategory, decimal Quantity, DateTime DeadlineUtc, string RegionCode);
public record RegisterSupplierCommand(string LegalName, string RegionCode);
public record QualifySupplierCommand(Guid SupplierId);
public record SubmitBidCommand(Guid OpportunityId, Guid SupplierId, decimal UnitPrice);
public record CloseOpportunityCommand(Guid OpportunityId);

