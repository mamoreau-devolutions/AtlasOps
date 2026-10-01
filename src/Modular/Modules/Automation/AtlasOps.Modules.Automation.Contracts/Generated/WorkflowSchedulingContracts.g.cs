namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum WorkflowSchedulingState { Draft, Active, Paused, Completed, Archived }
public sealed record WorkflowSchedulingRecord(Guid Id, string Name, string Owner, WorkflowSchedulingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record WorkflowSchedulingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record WorkflowSchedulingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record WorkflowSchedulingQuery(string? SearchText, WorkflowSchedulingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record WorkflowSchedulingPage(IReadOnlyList<WorkflowSchedulingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record WorkflowSchedulingMutation(bool Succeeded, string Code, string Message, WorkflowSchedulingRecord? Record, WorkflowSchedulingEvent? Event);
public interface IWorkflowSchedulingRepository
{
    ValueTask<WorkflowSchedulingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<WorkflowSchedulingPage> QueryAsync(WorkflowSchedulingQuery query, CancellationToken cancellationToken);
    ValueTask<WorkflowSchedulingMutation> SaveAsync(WorkflowSchedulingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IWorkflowSchedulingEventSink { ValueTask PublishAsync(WorkflowSchedulingEvent domainEvent, CancellationToken cancellationToken); }