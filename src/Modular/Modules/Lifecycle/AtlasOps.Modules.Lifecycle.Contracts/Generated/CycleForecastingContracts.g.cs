namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CycleForecastingState { Draft, Active, Paused, Completed, Archived }
public sealed record CycleForecastingRecord(Guid Id, string Name, string Owner, CycleForecastingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CycleForecastingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CycleForecastingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CycleForecastingQuery(string? SearchText, CycleForecastingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CycleForecastingPage(IReadOnlyList<CycleForecastingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CycleForecastingMutation(bool Succeeded, string Code, string Message, CycleForecastingRecord? Record, CycleForecastingEvent? Event);
public interface ICycleForecastingRepository
{
    ValueTask<CycleForecastingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CycleForecastingPage> QueryAsync(CycleForecastingQuery query, CancellationToken cancellationToken);
    ValueTask<CycleForecastingMutation> SaveAsync(CycleForecastingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICycleForecastingEventSink { ValueTask PublishAsync(CycleForecastingEvent domainEvent, CancellationToken cancellationToken); }