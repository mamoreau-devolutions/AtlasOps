namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DeviceDriftState { Draft, Active, Paused, Completed, Archived }
public sealed record DeviceDriftRecord(Guid Id, string Name, string Owner, DeviceDriftState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DeviceDriftCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DeviceDriftEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DeviceDriftQuery(string? SearchText, DeviceDriftState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DeviceDriftPage(IReadOnlyList<DeviceDriftRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DeviceDriftMutation(bool Succeeded, string Code, string Message, DeviceDriftRecord? Record, DeviceDriftEvent? Event);
public interface IDeviceDriftRepository
{
    ValueTask<DeviceDriftRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DeviceDriftPage> QueryAsync(DeviceDriftQuery query, CancellationToken cancellationToken);
    ValueTask<DeviceDriftMutation> SaveAsync(DeviceDriftRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDeviceDriftEventSink { ValueTask PublishAsync(DeviceDriftEvent domainEvent, CancellationToken cancellationToken); }