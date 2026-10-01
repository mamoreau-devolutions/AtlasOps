namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum WorkflowRetryState { Draft, Active, Paused, Completed, Archived }
public sealed record WorkflowRetryRecord(Guid Id, string Name, string Owner, WorkflowRetryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record WorkflowRetryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record WorkflowRetryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record WorkflowRetryQuery(string? SearchText, WorkflowRetryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record WorkflowRetryPage(IReadOnlyList<WorkflowRetryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record WorkflowRetryMutation(bool Succeeded, string Code, string Message, WorkflowRetryRecord? Record, WorkflowRetryEvent? Event);
public interface IWorkflowRetryRepository
{
    ValueTask<WorkflowRetryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<WorkflowRetryPage> QueryAsync(WorkflowRetryQuery query, CancellationToken cancellationToken);
    ValueTask<WorkflowRetryMutation> SaveAsync(WorkflowRetryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IWorkflowRetryEventSink { ValueTask PublishAsync(WorkflowRetryEvent domainEvent, CancellationToken cancellationToken); }