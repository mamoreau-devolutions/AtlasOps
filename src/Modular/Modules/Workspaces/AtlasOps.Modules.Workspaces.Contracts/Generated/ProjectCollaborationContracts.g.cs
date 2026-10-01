namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProjectCollaborationState { Draft, Active, Paused, Completed, Archived }
public sealed record ProjectCollaborationRecord(Guid Id, string Name, string Owner, ProjectCollaborationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProjectCollaborationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProjectCollaborationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProjectCollaborationQuery(string? SearchText, ProjectCollaborationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProjectCollaborationPage(IReadOnlyList<ProjectCollaborationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProjectCollaborationMutation(bool Succeeded, string Code, string Message, ProjectCollaborationRecord? Record, ProjectCollaborationEvent? Event);
public interface IProjectCollaborationRepository
{
    ValueTask<ProjectCollaborationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProjectCollaborationPage> QueryAsync(ProjectCollaborationQuery query, CancellationToken cancellationToken);
    ValueTask<ProjectCollaborationMutation> SaveAsync(ProjectCollaborationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProjectCollaborationEventSink { ValueTask PublishAsync(ProjectCollaborationEvent domainEvent, CancellationToken cancellationToken); }