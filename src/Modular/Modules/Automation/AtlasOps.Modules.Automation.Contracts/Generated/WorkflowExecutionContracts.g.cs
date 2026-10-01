namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum WorkflowExecutionState { Draft, Active, Paused, Completed, Archived }
public sealed record WorkflowExecutionRecord(Guid Id, string Name, string Owner, WorkflowExecutionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record WorkflowExecutionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record WorkflowExecutionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record WorkflowExecutionQuery(string? SearchText, WorkflowExecutionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record WorkflowExecutionPage(IReadOnlyList<WorkflowExecutionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record WorkflowExecutionMutation(bool Succeeded, string Code, string Message, WorkflowExecutionRecord? Record, WorkflowExecutionEvent? Event);
public interface IWorkflowExecutionRepository
{
    ValueTask<WorkflowExecutionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<WorkflowExecutionPage> QueryAsync(WorkflowExecutionQuery query, CancellationToken cancellationToken);
    ValueTask<WorkflowExecutionMutation> SaveAsync(WorkflowExecutionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IWorkflowExecutionEventSink { ValueTask PublishAsync(WorkflowExecutionEvent domainEvent, CancellationToken cancellationToken); }