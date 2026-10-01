namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum WorkflowCancellationState { Draft, Active, Paused, Completed, Archived }
public sealed record WorkflowCancellationRecord(Guid Id, string Name, string Owner, WorkflowCancellationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record WorkflowCancellationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record WorkflowCancellationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record WorkflowCancellationQuery(string? SearchText, WorkflowCancellationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record WorkflowCancellationPage(IReadOnlyList<WorkflowCancellationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record WorkflowCancellationMutation(bool Succeeded, string Code, string Message, WorkflowCancellationRecord? Record, WorkflowCancellationEvent? Event);
public interface IWorkflowCancellationRepository
{
    ValueTask<WorkflowCancellationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<WorkflowCancellationPage> QueryAsync(WorkflowCancellationQuery query, CancellationToken cancellationToken);
    ValueTask<WorkflowCancellationMutation> SaveAsync(WorkflowCancellationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IWorkflowCancellationEventSink { ValueTask PublishAsync(WorkflowCancellationEvent domainEvent, CancellationToken cancellationToken); }