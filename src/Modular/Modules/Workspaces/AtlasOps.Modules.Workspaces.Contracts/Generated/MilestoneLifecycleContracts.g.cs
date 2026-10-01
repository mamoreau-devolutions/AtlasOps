namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum MilestoneLifecycleState { Draft, Active, Paused, Completed, Archived }
public sealed record MilestoneLifecycleRecord(Guid Id, string Name, string Owner, MilestoneLifecycleState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record MilestoneLifecycleCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record MilestoneLifecycleEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record MilestoneLifecycleQuery(string? SearchText, MilestoneLifecycleState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record MilestoneLifecyclePage(IReadOnlyList<MilestoneLifecycleRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record MilestoneLifecycleMutation(bool Succeeded, string Code, string Message, MilestoneLifecycleRecord? Record, MilestoneLifecycleEvent? Event);
public interface IMilestoneLifecycleRepository
{
    ValueTask<MilestoneLifecycleRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<MilestoneLifecyclePage> QueryAsync(MilestoneLifecycleQuery query, CancellationToken cancellationToken);
    ValueTask<MilestoneLifecycleMutation> SaveAsync(MilestoneLifecycleRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IMilestoneLifecycleEventSink { ValueTask PublishAsync(MilestoneLifecycleEvent domainEvent, CancellationToken cancellationToken); }