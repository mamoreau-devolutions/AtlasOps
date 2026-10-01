namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RunbookTriageState { Draft, Active, Paused, Completed, Archived }
public sealed record RunbookTriageRecord(Guid Id, string Name, string Owner, RunbookTriageState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RunbookTriageCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RunbookTriageEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RunbookTriageQuery(string? SearchText, RunbookTriageState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RunbookTriagePage(IReadOnlyList<RunbookTriageRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RunbookTriageMutation(bool Succeeded, string Code, string Message, RunbookTriageRecord? Record, RunbookTriageEvent? Event);
public interface IRunbookTriageRepository
{
    ValueTask<RunbookTriageRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RunbookTriagePage> QueryAsync(RunbookTriageQuery query, CancellationToken cancellationToken);
    ValueTask<RunbookTriageMutation> SaveAsync(RunbookTriageRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRunbookTriageEventSink { ValueTask PublishAsync(RunbookTriageEvent domainEvent, CancellationToken cancellationToken); }