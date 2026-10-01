namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TraceReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record TraceReportingRecord(Guid Id, string Name, string Owner, TraceReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TraceReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TraceReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TraceReportingQuery(string? SearchText, TraceReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TraceReportingPage(IReadOnlyList<TraceReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TraceReportingMutation(bool Succeeded, string Code, string Message, TraceReportingRecord? Record, TraceReportingEvent? Event);
public interface ITraceReportingRepository
{
    ValueTask<TraceReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TraceReportingPage> QueryAsync(TraceReportingQuery query, CancellationToken cancellationToken);
    ValueTask<TraceReportingMutation> SaveAsync(TraceReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITraceReportingEventSink { ValueTask PublishAsync(TraceReportingEvent domainEvent, CancellationToken cancellationToken); }