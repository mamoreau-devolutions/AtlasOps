namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum StakeholderLifecycleState { Draft, Active, Paused, Completed, Archived }
public sealed record StakeholderLifecycleRecord(Guid Id, string Name, string Owner, StakeholderLifecycleState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record StakeholderLifecycleCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record StakeholderLifecycleEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record StakeholderLifecycleQuery(string? SearchText, StakeholderLifecycleState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record StakeholderLifecyclePage(IReadOnlyList<StakeholderLifecycleRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record StakeholderLifecycleMutation(bool Succeeded, string Code, string Message, StakeholderLifecycleRecord? Record, StakeholderLifecycleEvent? Event);
public interface IStakeholderLifecycleRepository
{
    ValueTask<StakeholderLifecycleRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<StakeholderLifecyclePage> QueryAsync(StakeholderLifecycleQuery query, CancellationToken cancellationToken);
    ValueTask<StakeholderLifecycleMutation> SaveAsync(StakeholderLifecycleRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IStakeholderLifecycleEventSink { ValueTask PublishAsync(StakeholderLifecycleEvent domainEvent, CancellationToken cancellationToken); }