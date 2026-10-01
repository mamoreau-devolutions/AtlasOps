namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DeviceOwnershipState { Draft, Active, Paused, Completed, Archived }
public sealed record DeviceOwnershipRecord(Guid Id, string Name, string Owner, DeviceOwnershipState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DeviceOwnershipCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DeviceOwnershipEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DeviceOwnershipQuery(string? SearchText, DeviceOwnershipState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DeviceOwnershipPage(IReadOnlyList<DeviceOwnershipRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DeviceOwnershipMutation(bool Succeeded, string Code, string Message, DeviceOwnershipRecord? Record, DeviceOwnershipEvent? Event);
public interface IDeviceOwnershipRepository
{
    ValueTask<DeviceOwnershipRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DeviceOwnershipPage> QueryAsync(DeviceOwnershipQuery query, CancellationToken cancellationToken);
    ValueTask<DeviceOwnershipMutation> SaveAsync(DeviceOwnershipRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDeviceOwnershipEventSink { ValueTask PublishAsync(DeviceOwnershipEvent domainEvent, CancellationToken cancellationToken); }