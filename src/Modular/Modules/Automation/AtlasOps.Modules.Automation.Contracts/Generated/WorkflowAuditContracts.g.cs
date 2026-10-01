namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum WorkflowAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record WorkflowAuditRecord(Guid Id, string Name, string Owner, WorkflowAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record WorkflowAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record WorkflowAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record WorkflowAuditQuery(string? SearchText, WorkflowAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record WorkflowAuditPage(IReadOnlyList<WorkflowAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record WorkflowAuditMutation(bool Succeeded, string Code, string Message, WorkflowAuditRecord? Record, WorkflowAuditEvent? Event);
public interface IWorkflowAuditRepository
{
    ValueTask<WorkflowAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<WorkflowAuditPage> QueryAsync(WorkflowAuditQuery query, CancellationToken cancellationToken);
    ValueTask<WorkflowAuditMutation> SaveAsync(WorkflowAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IWorkflowAuditEventSink { ValueTask PublishAsync(WorkflowAuditEvent domainEvent, CancellationToken cancellationToken); }