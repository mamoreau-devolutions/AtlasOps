namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TraceRetentionState { Draft, Active, Paused, Completed, Archived }
public sealed record TraceRetentionRecord(Guid Id, string Name, string Owner, TraceRetentionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TraceRetentionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TraceRetentionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TraceRetentionQuery(string? SearchText, TraceRetentionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TraceRetentionPage(IReadOnlyList<TraceRetentionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TraceRetentionMutation(bool Succeeded, string Code, string Message, TraceRetentionRecord? Record, TraceRetentionEvent? Event);
public interface ITraceRetentionRepository
{
    ValueTask<TraceRetentionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TraceRetentionPage> QueryAsync(TraceRetentionQuery query, CancellationToken cancellationToken);
    ValueTask<TraceRetentionMutation> SaveAsync(TraceRetentionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITraceRetentionEventSink { ValueTask PublishAsync(TraceRetentionEvent domainEvent, CancellationToken cancellationToken); }