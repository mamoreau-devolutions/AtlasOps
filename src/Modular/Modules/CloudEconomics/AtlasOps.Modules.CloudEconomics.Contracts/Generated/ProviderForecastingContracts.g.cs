namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProviderForecastingState { Draft, Active, Paused, Completed, Archived }
public sealed record ProviderForecastingRecord(Guid Id, string Name, string Owner, ProviderForecastingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProviderForecastingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProviderForecastingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProviderForecastingQuery(string? SearchText, ProviderForecastingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProviderForecastingPage(IReadOnlyList<ProviderForecastingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProviderForecastingMutation(bool Succeeded, string Code, string Message, ProviderForecastingRecord? Record, ProviderForecastingEvent? Event);
public interface IProviderForecastingRepository
{
    ValueTask<ProviderForecastingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProviderForecastingPage> QueryAsync(ProviderForecastingQuery query, CancellationToken cancellationToken);
    ValueTask<ProviderForecastingMutation> SaveAsync(ProviderForecastingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProviderForecastingEventSink { ValueTask PublishAsync(ProviderForecastingEvent domainEvent, CancellationToken cancellationToken); }