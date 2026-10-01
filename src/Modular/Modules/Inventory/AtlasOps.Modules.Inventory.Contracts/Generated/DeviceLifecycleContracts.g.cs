namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DeviceLifecycleState { Draft, Active, Paused, Completed, Archived }
public sealed record DeviceLifecycleRecord(Guid Id, string Name, string Owner, DeviceLifecycleState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DeviceLifecycleCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DeviceLifecycleEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DeviceLifecycleQuery(string? SearchText, DeviceLifecycleState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DeviceLifecyclePage(IReadOnlyList<DeviceLifecycleRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DeviceLifecycleMutation(bool Succeeded, string Code, string Message, DeviceLifecycleRecord? Record, DeviceLifecycleEvent? Event);
public interface IDeviceLifecycleRepository
{
    ValueTask<DeviceLifecycleRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DeviceLifecyclePage> QueryAsync(DeviceLifecycleQuery query, CancellationToken cancellationToken);
    ValueTask<DeviceLifecycleMutation> SaveAsync(DeviceLifecycleRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDeviceLifecycleEventSink { ValueTask PublishAsync(DeviceLifecycleEvent domainEvent, CancellationToken cancellationToken); }