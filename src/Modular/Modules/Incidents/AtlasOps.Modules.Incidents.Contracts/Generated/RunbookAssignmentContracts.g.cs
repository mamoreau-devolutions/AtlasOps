namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RunbookAssignmentState { Draft, Active, Paused, Completed, Archived }
public sealed record RunbookAssignmentRecord(Guid Id, string Name, string Owner, RunbookAssignmentState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RunbookAssignmentCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RunbookAssignmentEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RunbookAssignmentQuery(string? SearchText, RunbookAssignmentState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RunbookAssignmentPage(IReadOnlyList<RunbookAssignmentRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RunbookAssignmentMutation(bool Succeeded, string Code, string Message, RunbookAssignmentRecord? Record, RunbookAssignmentEvent? Event);
public interface IRunbookAssignmentRepository
{
    ValueTask<RunbookAssignmentRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RunbookAssignmentPage> QueryAsync(RunbookAssignmentQuery query, CancellationToken cancellationToken);
    ValueTask<RunbookAssignmentMutation> SaveAsync(RunbookAssignmentRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRunbookAssignmentEventSink { ValueTask PublishAsync(RunbookAssignmentEvent domainEvent, CancellationToken cancellationToken); }