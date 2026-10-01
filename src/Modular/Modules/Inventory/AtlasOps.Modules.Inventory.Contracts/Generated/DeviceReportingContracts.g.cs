namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DeviceReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record DeviceReportingRecord(Guid Id, string Name, string Owner, DeviceReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DeviceReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DeviceReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DeviceReportingQuery(string? SearchText, DeviceReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DeviceReportingPage(IReadOnlyList<DeviceReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DeviceReportingMutation(bool Succeeded, string Code, string Message, DeviceReportingRecord? Record, DeviceReportingEvent? Event);
public interface IDeviceReportingRepository
{
    ValueTask<DeviceReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DeviceReportingPage> QueryAsync(DeviceReportingQuery query, CancellationToken cancellationToken);
    ValueTask<DeviceReportingMutation> SaveAsync(DeviceReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDeviceReportingEventSink { ValueTask PublishAsync(DeviceReportingEvent domainEvent, CancellationToken cancellationToken); }