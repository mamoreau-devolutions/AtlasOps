namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LogAggregationState { Draft, Active, Paused, Completed, Archived }
public sealed record LogAggregationRecord(Guid Id, string Name, string Owner, LogAggregationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LogAggregationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LogAggregationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LogAggregationQuery(string? SearchText, LogAggregationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LogAggregationPage(IReadOnlyList<LogAggregationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LogAggregationMutation(bool Succeeded, string Code, string Message, LogAggregationRecord? Record, LogAggregationEvent? Event);
public interface ILogAggregationRepository
{
    ValueTask<LogAggregationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LogAggregationPage> QueryAsync(LogAggregationQuery query, CancellationToken cancellationToken);
    ValueTask<LogAggregationMutation> SaveAsync(LogAggregationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILogAggregationEventSink { ValueTask PublishAsync(LogAggregationEvent domainEvent, CancellationToken cancellationToken); }