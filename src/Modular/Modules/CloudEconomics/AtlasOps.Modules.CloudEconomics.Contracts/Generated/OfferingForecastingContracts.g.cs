namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum OfferingForecastingState { Draft, Active, Paused, Completed, Archived }
public sealed record OfferingForecastingRecord(Guid Id, string Name, string Owner, OfferingForecastingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record OfferingForecastingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record OfferingForecastingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record OfferingForecastingQuery(string? SearchText, OfferingForecastingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record OfferingForecastingPage(IReadOnlyList<OfferingForecastingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record OfferingForecastingMutation(bool Succeeded, string Code, string Message, OfferingForecastingRecord? Record, OfferingForecastingEvent? Event);
public interface IOfferingForecastingRepository
{
    ValueTask<OfferingForecastingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<OfferingForecastingPage> QueryAsync(OfferingForecastingQuery query, CancellationToken cancellationToken);
    ValueTask<OfferingForecastingMutation> SaveAsync(OfferingForecastingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IOfferingForecastingEventSink { ValueTask PublishAsync(OfferingForecastingEvent domainEvent, CancellationToken cancellationToken); }