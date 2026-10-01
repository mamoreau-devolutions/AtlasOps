namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DeviceInventoryState { Draft, Active, Paused, Completed, Archived }
public sealed record DeviceInventoryRecord(Guid Id, string Name, string Owner, DeviceInventoryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DeviceInventoryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DeviceInventoryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DeviceInventoryQuery(string? SearchText, DeviceInventoryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DeviceInventoryPage(IReadOnlyList<DeviceInventoryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DeviceInventoryMutation(bool Succeeded, string Code, string Message, DeviceInventoryRecord? Record, DeviceInventoryEvent? Event);
public interface IDeviceInventoryRepository
{
    ValueTask<DeviceInventoryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DeviceInventoryPage> QueryAsync(DeviceInventoryQuery query, CancellationToken cancellationToken);
    ValueTask<DeviceInventoryMutation> SaveAsync(DeviceInventoryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDeviceInventoryEventSink { ValueTask PublishAsync(DeviceInventoryEvent domainEvent, CancellationToken cancellationToken); }