namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DivisionSearchState { Draft, Active, Paused, Completed, Archived }
public sealed record DivisionSearchRecord(Guid Id, string Name, string Owner, DivisionSearchState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DivisionSearchCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DivisionSearchEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DivisionSearchQuery(string? SearchText, DivisionSearchState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DivisionSearchPage(IReadOnlyList<DivisionSearchRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DivisionSearchMutation(bool Succeeded, string Code, string Message, DivisionSearchRecord? Record, DivisionSearchEvent? Event);
public interface IDivisionSearchRepository
{
    ValueTask<DivisionSearchRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DivisionSearchPage> QueryAsync(DivisionSearchQuery query, CancellationToken cancellationToken);
    ValueTask<DivisionSearchMutation> SaveAsync(DivisionSearchRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDivisionSearchEventSink { ValueTask PublishAsync(DivisionSearchEvent domainEvent, CancellationToken cancellationToken); }