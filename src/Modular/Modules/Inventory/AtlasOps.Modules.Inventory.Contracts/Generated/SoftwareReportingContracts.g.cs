namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum SoftwareReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record SoftwareReportingRecord(Guid Id, string Name, string Owner, SoftwareReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record SoftwareReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SoftwareReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record SoftwareReportingQuery(string? SearchText, SoftwareReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record SoftwareReportingPage(IReadOnlyList<SoftwareReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record SoftwareReportingMutation(bool Succeeded, string Code, string Message, SoftwareReportingRecord? Record, SoftwareReportingEvent? Event);
public interface ISoftwareReportingRepository
{
    ValueTask<SoftwareReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<SoftwareReportingPage> QueryAsync(SoftwareReportingQuery query, CancellationToken cancellationToken);
    ValueTask<SoftwareReportingMutation> SaveAsync(SoftwareReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ISoftwareReportingEventSink { ValueTask PublishAsync(SoftwareReportingEvent domainEvent, CancellationToken cancellationToken); }