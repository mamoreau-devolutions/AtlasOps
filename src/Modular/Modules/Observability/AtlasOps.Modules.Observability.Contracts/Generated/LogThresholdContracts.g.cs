namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LogThresholdState { Draft, Active, Paused, Completed, Archived }
public sealed record LogThresholdRecord(Guid Id, string Name, string Owner, LogThresholdState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LogThresholdCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LogThresholdEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LogThresholdQuery(string? SearchText, LogThresholdState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LogThresholdPage(IReadOnlyList<LogThresholdRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LogThresholdMutation(bool Succeeded, string Code, string Message, LogThresholdRecord? Record, LogThresholdEvent? Event);
public interface ILogThresholdRepository
{
    ValueTask<LogThresholdRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LogThresholdPage> QueryAsync(LogThresholdQuery query, CancellationToken cancellationToken);
    ValueTask<LogThresholdMutation> SaveAsync(LogThresholdRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILogThresholdEventSink { ValueTask PublishAsync(LogThresholdEvent domainEvent, CancellationToken cancellationToken); }