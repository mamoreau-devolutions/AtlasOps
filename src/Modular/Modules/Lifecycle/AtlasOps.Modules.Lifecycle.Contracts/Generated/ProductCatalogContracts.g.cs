namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProductCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record ProductCatalogRecord(Guid Id, string Name, string Owner, ProductCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProductCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProductCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProductCatalogQuery(string? SearchText, ProductCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProductCatalogPage(IReadOnlyList<ProductCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProductCatalogMutation(bool Succeeded, string Code, string Message, ProductCatalogRecord? Record, ProductCatalogEvent? Event);
public interface IProductCatalogRepository
{
    ValueTask<ProductCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProductCatalogPage> QueryAsync(ProductCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<ProductCatalogMutation> SaveAsync(ProductCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProductCatalogEventSink { ValueTask PublishAsync(ProductCatalogEvent domainEvent, CancellationToken cancellationToken); }