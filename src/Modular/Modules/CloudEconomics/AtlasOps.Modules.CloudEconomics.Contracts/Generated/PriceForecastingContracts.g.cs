namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PriceForecastingState { Draft, Active, Paused, Completed, Archived }
public sealed record PriceForecastingRecord(Guid Id, string Name, string Owner, PriceForecastingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PriceForecastingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PriceForecastingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PriceForecastingQuery(string? SearchText, PriceForecastingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PriceForecastingPage(IReadOnlyList<PriceForecastingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PriceForecastingMutation(bool Succeeded, string Code, string Message, PriceForecastingRecord? Record, PriceForecastingEvent? Event);
public interface IPriceForecastingRepository
{
    ValueTask<PriceForecastingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PriceForecastingPage> QueryAsync(PriceForecastingQuery query, CancellationToken cancellationToken);
    ValueTask<PriceForecastingMutation> SaveAsync(PriceForecastingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPriceForecastingEventSink { ValueTask PublishAsync(PriceForecastingEvent domainEvent, CancellationToken cancellationToken); }