namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LogCorrelationState { Draft, Active, Paused, Completed, Archived }
public sealed record LogCorrelationRecord(Guid Id, string Name, string Owner, LogCorrelationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LogCorrelationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LogCorrelationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LogCorrelationQuery(string? SearchText, LogCorrelationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LogCorrelationPage(IReadOnlyList<LogCorrelationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LogCorrelationMutation(bool Succeeded, string Code, string Message, LogCorrelationRecord? Record, LogCorrelationEvent? Event);
public interface ILogCorrelationRepository
{
    ValueTask<LogCorrelationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LogCorrelationPage> QueryAsync(LogCorrelationQuery query, CancellationToken cancellationToken);
    ValueTask<LogCorrelationMutation> SaveAsync(LogCorrelationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILogCorrelationEventSink { ValueTask PublishAsync(LogCorrelationEvent domainEvent, CancellationToken cancellationToken); }