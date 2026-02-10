using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Abstractions;
using Procurement.Domain.Bids;
using Procurement.Domain.Common;
using Procurement.Domain.Opportunities;
using Procurement.Domain.Suppliers;
using Procurement.Infrastructure.Outbox;

namespace Procurement.Infrastructure.Persistence;

public sealed class ProcurementDbContext : DbContext, IProcurementDbContext
{
    public ProcurementDbContext(DbContextOptions<ProcurementDbContext> options) : base(options) { }

    // EF DbSets (internal use)
    public DbSet<Opportunity> OpportunitiesSet => Set<Opportunity>();
    public DbSet<Supplier> SuppliersSet => Set<Supplier>();
    public DbSet<Bid> BidsSet => Set<Bid>();

    public DbSet<OutboxMessage> Outbox => Set<OutboxMessage>();

    // Interface exposure (IQueryable)
    public IQueryable<Opportunity> Opportunities => OpportunitiesSet;
    public IQueryable<Supplier> Suppliers => SuppliersSet;
    public IQueryable<Bid> Bids => BidsSet;

    public void AddOpportunity(Opportunity opportunity) => OpportunitiesSet.Add(opportunity);
    public void AddSupplier(Supplier supplier) => SuppliersSet.Add(supplier);
    public void AddBid(Bid bid) => BidsSet.Add(bid);

    public Task<Supplier?> FindSupplier(Guid id, CancellationToken ct)
        => SuppliersSet.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Opportunity?> FindOpportunity(Guid id, CancellationToken ct)
        => OpportunitiesSet.FirstOrDefaultAsync(x => x.Id == id, ct);

   protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Ignore<DomainEvent>();

    modelBuilder.Entity<Opportunity>().HasKey(x => x.Id);
    modelBuilder.Entity<Supplier>().HasKey(x => x.Id);
    modelBuilder.Entity<Bid>().HasKey(x => x.Id);

    modelBuilder.Entity<Opportunity>().Ignore(e => e.DomainEvents);
    modelBuilder.Entity<Supplier>().Ignore(e => e.DomainEvents);
    modelBuilder.Entity<Bid>().Ignore(e => e.DomainEvents);

    modelBuilder.Entity<OutboxMessage>().HasKey(x => x.Id);
    modelBuilder.Entity<OutboxMessage>().Property(x => x.Type).HasMaxLength(300);
}


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker.Entries<Entity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = entities.SelectMany(e => e.DomainEvents).ToList();
        entities.ForEach(e => e.ClearDomainEvents());

        foreach (var ev in domainEvents)
        {
            Outbox.Add(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredAtUtc = ev.OccurredAtUtc,
                Type = ev.GetType().FullName!,
                Payload = JsonSerializer.Serialize(ev, ev.GetType()),
            });
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
