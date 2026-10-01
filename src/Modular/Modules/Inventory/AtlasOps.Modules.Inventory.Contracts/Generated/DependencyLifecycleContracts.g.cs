namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DependencyLifecycleState { Draft, Active, Paused, Completed, Archived }
public sealed record DependencyLifecycleRecord(Guid Id, string Name, string Owner, DependencyLifecycleState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DependencyLifecycleCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DependencyLifecycleEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DependencyLifecycleQuery(string? SearchText, DependencyLifecycleState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DependencyLifecyclePage(IReadOnlyList<DependencyLifecycleRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DependencyLifecycleMutation(bool Succeeded, string Code, string Message, DependencyLifecycleRecord? Record, DependencyLifecycleEvent? Event);
public interface IDependencyLifecycleRepository
{
    ValueTask<DependencyLifecycleRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DependencyLifecyclePage> QueryAsync(DependencyLifecycleQuery query, CancellationToken cancellationToken);
    ValueTask<DependencyLifecycleMutation> SaveAsync(DependencyLifecycleRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDependencyLifecycleEventSink { ValueTask PublishAsync(DependencyLifecycleEvent domainEvent, CancellationToken cancellationToken); }