namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum FindingCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record FindingCatalogRecord(Guid Id, string Name, string Owner, FindingCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record FindingCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record FindingCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record FindingCatalogQuery(string? SearchText, FindingCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record FindingCatalogPage(IReadOnlyList<FindingCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record FindingCatalogMutation(bool Succeeded, string Code, string Message, FindingCatalogRecord? Record, FindingCatalogEvent? Event);
public interface IFindingCatalogRepository
{
    ValueTask<FindingCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<FindingCatalogPage> QueryAsync(FindingCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<FindingCatalogMutation> SaveAsync(FindingCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IFindingCatalogEventSink { ValueTask PublishAsync(FindingCatalogEvent domainEvent, CancellationToken cancellationToken); }