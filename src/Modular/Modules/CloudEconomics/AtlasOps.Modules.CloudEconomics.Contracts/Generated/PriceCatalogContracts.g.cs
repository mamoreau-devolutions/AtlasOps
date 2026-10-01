namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PriceCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record PriceCatalogRecord(Guid Id, string Name, string Owner, PriceCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PriceCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PriceCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PriceCatalogQuery(string? SearchText, PriceCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PriceCatalogPage(IReadOnlyList<PriceCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PriceCatalogMutation(bool Succeeded, string Code, string Message, PriceCatalogRecord? Record, PriceCatalogEvent? Event);
public interface IPriceCatalogRepository
{
    ValueTask<PriceCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PriceCatalogPage> QueryAsync(PriceCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<PriceCatalogMutation> SaveAsync(PriceCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPriceCatalogEventSink { ValueTask PublishAsync(PriceCatalogEvent domainEvent, CancellationToken cancellationToken); }