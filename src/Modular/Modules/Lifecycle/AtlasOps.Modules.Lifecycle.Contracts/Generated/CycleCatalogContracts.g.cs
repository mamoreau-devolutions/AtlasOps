namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CycleCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record CycleCatalogRecord(Guid Id, string Name, string Owner, CycleCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CycleCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CycleCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CycleCatalogQuery(string? SearchText, CycleCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CycleCatalogPage(IReadOnlyList<CycleCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CycleCatalogMutation(bool Succeeded, string Code, string Message, CycleCatalogRecord? Record, CycleCatalogEvent? Event);
public interface ICycleCatalogRepository
{
    ValueTask<CycleCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CycleCatalogPage> QueryAsync(CycleCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<CycleCatalogMutation> SaveAsync(CycleCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICycleCatalogEventSink { ValueTask PublishAsync(CycleCatalogEvent domainEvent, CancellationToken cancellationToken); }