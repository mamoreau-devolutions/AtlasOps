namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ReleaseReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record ReleaseReportingRecord(Guid Id, string Name, string Owner, ReleaseReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ReleaseReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ReleaseReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ReleaseReportingQuery(string? SearchText, ReleaseReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ReleaseReportingPage(IReadOnlyList<ReleaseReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ReleaseReportingMutation(bool Succeeded, string Code, string Message, ReleaseReportingRecord? Record, ReleaseReportingEvent? Event);
public interface IReleaseReportingRepository
{
    ValueTask<ReleaseReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ReleaseReportingPage> QueryAsync(ReleaseReportingQuery query, CancellationToken cancellationToken);
    ValueTask<ReleaseReportingMutation> SaveAsync(ReleaseReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IReleaseReportingEventSink { ValueTask PublishAsync(ReleaseReportingEvent domainEvent, CancellationToken cancellationToken); }