namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DeviceClassificationState { Draft, Active, Paused, Completed, Archived }
public sealed record DeviceClassificationRecord(Guid Id, string Name, string Owner, DeviceClassificationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DeviceClassificationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DeviceClassificationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DeviceClassificationQuery(string? SearchText, DeviceClassificationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DeviceClassificationPage(IReadOnlyList<DeviceClassificationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DeviceClassificationMutation(bool Succeeded, string Code, string Message, DeviceClassificationRecord? Record, DeviceClassificationEvent? Event);
public interface IDeviceClassificationRepository
{
    ValueTask<DeviceClassificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DeviceClassificationPage> QueryAsync(DeviceClassificationQuery query, CancellationToken cancellationToken);
    ValueTask<DeviceClassificationMutation> SaveAsync(DeviceClassificationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDeviceClassificationEventSink { ValueTask PublishAsync(DeviceClassificationEvent domainEvent, CancellationToken cancellationToken); }