namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ReleaseForecastingState { Draft, Active, Paused, Completed, Archived }
public sealed record ReleaseForecastingRecord(Guid Id, string Name, string Owner, ReleaseForecastingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ReleaseForecastingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ReleaseForecastingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ReleaseForecastingQuery(string? SearchText, ReleaseForecastingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ReleaseForecastingPage(IReadOnlyList<ReleaseForecastingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ReleaseForecastingMutation(bool Succeeded, string Code, string Message, ReleaseForecastingRecord? Record, ReleaseForecastingEvent? Event);
public interface IReleaseForecastingRepository
{
    ValueTask<ReleaseForecastingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ReleaseForecastingPage> QueryAsync(ReleaseForecastingQuery query, CancellationToken cancellationToken);
    ValueTask<ReleaseForecastingMutation> SaveAsync(ReleaseForecastingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IReleaseForecastingEventSink { ValueTask PublishAsync(ReleaseForecastingEvent domainEvent, CancellationToken cancellationToken); }