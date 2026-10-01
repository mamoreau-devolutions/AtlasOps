namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum OfferingCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record OfferingCatalogRecord(Guid Id, string Name, string Owner, OfferingCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record OfferingCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record OfferingCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record OfferingCatalogQuery(string? SearchText, OfferingCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record OfferingCatalogPage(IReadOnlyList<OfferingCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record OfferingCatalogMutation(bool Succeeded, string Code, string Message, OfferingCatalogRecord? Record, OfferingCatalogEvent? Event);
public interface IOfferingCatalogRepository
{
    ValueTask<OfferingCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<OfferingCatalogPage> QueryAsync(OfferingCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<OfferingCatalogMutation> SaveAsync(OfferingCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IOfferingCatalogEventSink { ValueTask PublishAsync(OfferingCatalogEvent domainEvent, CancellationToken cancellationToken); }