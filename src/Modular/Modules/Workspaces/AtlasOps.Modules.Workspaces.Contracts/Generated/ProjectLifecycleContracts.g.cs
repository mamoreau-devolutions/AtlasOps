namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProjectLifecycleState { Draft, Active, Paused, Completed, Archived }
public sealed record ProjectLifecycleRecord(Guid Id, string Name, string Owner, ProjectLifecycleState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProjectLifecycleCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProjectLifecycleEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProjectLifecycleQuery(string? SearchText, ProjectLifecycleState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProjectLifecyclePage(IReadOnlyList<ProjectLifecycleRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProjectLifecycleMutation(bool Succeeded, string Code, string Message, ProjectLifecycleRecord? Record, ProjectLifecycleEvent? Event);
public interface IProjectLifecycleRepository
{
    ValueTask<ProjectLifecycleRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProjectLifecyclePage> QueryAsync(ProjectLifecycleQuery query, CancellationToken cancellationToken);
    ValueTask<ProjectLifecycleMutation> SaveAsync(ProjectLifecycleRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProjectLifecycleEventSink { ValueTask PublishAsync(ProjectLifecycleEvent domainEvent, CancellationToken cancellationToken); }