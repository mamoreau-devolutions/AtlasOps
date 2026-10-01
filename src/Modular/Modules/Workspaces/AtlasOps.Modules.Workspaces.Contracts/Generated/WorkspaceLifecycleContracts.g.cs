namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum WorkspaceLifecycleState { Draft, Active, Paused, Completed, Archived }
public sealed record WorkspaceLifecycleRecord(Guid Id, string Name, string Owner, WorkspaceLifecycleState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record WorkspaceLifecycleCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record WorkspaceLifecycleEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record WorkspaceLifecycleQuery(string? SearchText, WorkspaceLifecycleState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record WorkspaceLifecyclePage(IReadOnlyList<WorkspaceLifecycleRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record WorkspaceLifecycleMutation(bool Succeeded, string Code, string Message, WorkspaceLifecycleRecord? Record, WorkspaceLifecycleEvent? Event);
public interface IWorkspaceLifecycleRepository
{
    ValueTask<WorkspaceLifecycleRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<WorkspaceLifecyclePage> QueryAsync(WorkspaceLifecycleQuery query, CancellationToken cancellationToken);
    ValueTask<WorkspaceLifecycleMutation> SaveAsync(WorkspaceLifecycleRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IWorkspaceLifecycleEventSink { ValueTask PublishAsync(WorkspaceLifecycleEvent domainEvent, CancellationToken cancellationToken); }