namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum VersionReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record VersionReportingRecord(Guid Id, string Name, string Owner, VersionReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record VersionReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record VersionReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record VersionReportingQuery(string? SearchText, VersionReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record VersionReportingPage(IReadOnlyList<VersionReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record VersionReportingMutation(bool Succeeded, string Code, string Message, VersionReportingRecord? Record, VersionReportingEvent? Event);
public interface IVersionReportingRepository
{
    ValueTask<VersionReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<VersionReportingPage> QueryAsync(VersionReportingQuery query, CancellationToken cancellationToken);
    ValueTask<VersionReportingMutation> SaveAsync(VersionReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IVersionReportingEventSink { ValueTask PublishAsync(VersionReportingEvent domainEvent, CancellationToken cancellationToken); }