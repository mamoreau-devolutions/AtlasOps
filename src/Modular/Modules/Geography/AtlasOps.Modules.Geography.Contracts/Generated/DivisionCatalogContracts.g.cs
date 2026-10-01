namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DivisionCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record DivisionCatalogRecord(Guid Id, string Name, string Owner, DivisionCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DivisionCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DivisionCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DivisionCatalogQuery(string? SearchText, DivisionCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DivisionCatalogPage(IReadOnlyList<DivisionCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DivisionCatalogMutation(bool Succeeded, string Code, string Message, DivisionCatalogRecord? Record, DivisionCatalogEvent? Event);
public interface IDivisionCatalogRepository
{
    ValueTask<DivisionCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DivisionCatalogPage> QueryAsync(DivisionCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<DivisionCatalogMutation> SaveAsync(DivisionCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDivisionCatalogEventSink { ValueTask PublishAsync(DivisionCatalogEvent domainEvent, CancellationToken cancellationToken); }