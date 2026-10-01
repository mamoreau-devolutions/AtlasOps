namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RunbookDetectionState { Draft, Active, Paused, Completed, Archived }
public sealed record RunbookDetectionRecord(Guid Id, string Name, string Owner, RunbookDetectionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RunbookDetectionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RunbookDetectionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RunbookDetectionQuery(string? SearchText, RunbookDetectionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RunbookDetectionPage(IReadOnlyList<RunbookDetectionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RunbookDetectionMutation(bool Succeeded, string Code, string Message, RunbookDetectionRecord? Record, RunbookDetectionEvent? Event);
public interface IRunbookDetectionRepository
{
    ValueTask<RunbookDetectionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RunbookDetectionPage> QueryAsync(RunbookDetectionQuery query, CancellationToken cancellationToken);
    ValueTask<RunbookDetectionMutation> SaveAsync(RunbookDetectionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRunbookDetectionEventSink { ValueTask PublishAsync(RunbookDetectionEvent domainEvent, CancellationToken cancellationToken); }