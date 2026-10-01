namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TraceThresholdState { Draft, Active, Paused, Completed, Archived }
public sealed record TraceThresholdRecord(Guid Id, string Name, string Owner, TraceThresholdState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TraceThresholdCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TraceThresholdEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TraceThresholdQuery(string? SearchText, TraceThresholdState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TraceThresholdPage(IReadOnlyList<TraceThresholdRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TraceThresholdMutation(bool Succeeded, string Code, string Message, TraceThresholdRecord? Record, TraceThresholdEvent? Event);
public interface ITraceThresholdRepository
{
    ValueTask<TraceThresholdRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TraceThresholdPage> QueryAsync(TraceThresholdQuery query, CancellationToken cancellationToken);
    ValueTask<TraceThresholdMutation> SaveAsync(TraceThresholdRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITraceThresholdEventSink { ValueTask PublishAsync(TraceThresholdEvent domainEvent, CancellationToken cancellationToken); }