namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DivisionComparisonState { Draft, Active, Paused, Completed, Archived }
public sealed record DivisionComparisonRecord(Guid Id, string Name, string Owner, DivisionComparisonState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DivisionComparisonCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DivisionComparisonEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DivisionComparisonQuery(string? SearchText, DivisionComparisonState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DivisionComparisonPage(IReadOnlyList<DivisionComparisonRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DivisionComparisonMutation(bool Succeeded, string Code, string Message, DivisionComparisonRecord? Record, DivisionComparisonEvent? Event);
public interface IDivisionComparisonRepository
{
    ValueTask<DivisionComparisonRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DivisionComparisonPage> QueryAsync(DivisionComparisonQuery query, CancellationToken cancellationToken);
    ValueTask<DivisionComparisonMutation> SaveAsync(DivisionComparisonRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDivisionComparisonEventSink { ValueTask PublishAsync(DivisionComparisonEvent domainEvent, CancellationToken cancellationToken); }