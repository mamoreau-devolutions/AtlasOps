namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DiscoveryLifecycleState { Draft, Active, Paused, Completed, Archived }
public sealed record DiscoveryLifecycleRecord(Guid Id, string Name, string Owner, DiscoveryLifecycleState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DiscoveryLifecycleCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DiscoveryLifecycleEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DiscoveryLifecycleQuery(string? SearchText, DiscoveryLifecycleState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DiscoveryLifecyclePage(IReadOnlyList<DiscoveryLifecycleRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DiscoveryLifecycleMutation(bool Succeeded, string Code, string Message, DiscoveryLifecycleRecord? Record, DiscoveryLifecycleEvent? Event);
public interface IDiscoveryLifecycleRepository
{
    ValueTask<DiscoveryLifecycleRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DiscoveryLifecyclePage> QueryAsync(DiscoveryLifecycleQuery query, CancellationToken cancellationToken);
    ValueTask<DiscoveryLifecycleMutation> SaveAsync(DiscoveryLifecycleRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDiscoveryLifecycleEventSink { ValueTask PublishAsync(DiscoveryLifecycleEvent domainEvent, CancellationToken cancellationToken); }