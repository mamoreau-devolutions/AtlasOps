namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CycleReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record CycleReportingRecord(Guid Id, string Name, string Owner, CycleReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CycleReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CycleReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CycleReportingQuery(string? SearchText, CycleReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CycleReportingPage(IReadOnlyList<CycleReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CycleReportingMutation(bool Succeeded, string Code, string Message, CycleReportingRecord? Record, CycleReportingEvent? Event);
public interface ICycleReportingRepository
{
    ValueTask<CycleReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CycleReportingPage> QueryAsync(CycleReportingQuery query, CancellationToken cancellationToken);
    ValueTask<CycleReportingMutation> SaveAsync(CycleReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICycleReportingEventSink { ValueTask PublishAsync(CycleReportingEvent domainEvent, CancellationToken cancellationToken); }