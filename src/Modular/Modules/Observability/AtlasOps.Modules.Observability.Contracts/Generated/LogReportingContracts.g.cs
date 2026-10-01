namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LogReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record LogReportingRecord(Guid Id, string Name, string Owner, LogReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LogReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LogReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LogReportingQuery(string? SearchText, LogReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LogReportingPage(IReadOnlyList<LogReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LogReportingMutation(bool Succeeded, string Code, string Message, LogReportingRecord? Record, LogReportingEvent? Event);
public interface ILogReportingRepository
{
    ValueTask<LogReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LogReportingPage> QueryAsync(LogReportingQuery query, CancellationToken cancellationToken);
    ValueTask<LogReportingMutation> SaveAsync(LogReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILogReportingEventSink { ValueTask PublishAsync(LogReportingEvent domainEvent, CancellationToken cancellationToken); }