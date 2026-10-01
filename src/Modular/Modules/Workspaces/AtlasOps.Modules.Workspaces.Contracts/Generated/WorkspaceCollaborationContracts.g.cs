namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum WorkspaceCollaborationState { Draft, Active, Paused, Completed, Archived }
public sealed record WorkspaceCollaborationRecord(Guid Id, string Name, string Owner, WorkspaceCollaborationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record WorkspaceCollaborationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record WorkspaceCollaborationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record WorkspaceCollaborationQuery(string? SearchText, WorkspaceCollaborationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record WorkspaceCollaborationPage(IReadOnlyList<WorkspaceCollaborationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record WorkspaceCollaborationMutation(bool Succeeded, string Code, string Message, WorkspaceCollaborationRecord? Record, WorkspaceCollaborationEvent? Event);
public interface IWorkspaceCollaborationRepository
{
    ValueTask<WorkspaceCollaborationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<WorkspaceCollaborationPage> QueryAsync(WorkspaceCollaborationQuery query, CancellationToken cancellationToken);
    ValueTask<WorkspaceCollaborationMutation> SaveAsync(WorkspaceCollaborationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IWorkspaceCollaborationEventSink { ValueTask PublishAsync(WorkspaceCollaborationEvent domainEvent, CancellationToken cancellationToken); }