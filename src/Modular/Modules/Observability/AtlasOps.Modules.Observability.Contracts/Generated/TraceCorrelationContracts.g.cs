namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TraceCorrelationState { Draft, Active, Paused, Completed, Archived }
public sealed record TraceCorrelationRecord(Guid Id, string Name, string Owner, TraceCorrelationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TraceCorrelationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TraceCorrelationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TraceCorrelationQuery(string? SearchText, TraceCorrelationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TraceCorrelationPage(IReadOnlyList<TraceCorrelationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TraceCorrelationMutation(bool Succeeded, string Code, string Message, TraceCorrelationRecord? Record, TraceCorrelationEvent? Event);
public interface ITraceCorrelationRepository
{
    ValueTask<TraceCorrelationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TraceCorrelationPage> QueryAsync(TraceCorrelationQuery query, CancellationToken cancellationToken);
    ValueTask<TraceCorrelationMutation> SaveAsync(TraceCorrelationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITraceCorrelationEventSink { ValueTask PublishAsync(TraceCorrelationEvent domainEvent, CancellationToken cancellationToken); }