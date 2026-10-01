namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProductForecastingState { Draft, Active, Paused, Completed, Archived }
public sealed record ProductForecastingRecord(Guid Id, string Name, string Owner, ProductForecastingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProductForecastingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProductForecastingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProductForecastingQuery(string? SearchText, ProductForecastingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProductForecastingPage(IReadOnlyList<ProductForecastingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProductForecastingMutation(bool Succeeded, string Code, string Message, ProductForecastingRecord? Record, ProductForecastingEvent? Event);
public interface IProductForecastingRepository
{
    ValueTask<ProductForecastingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProductForecastingPage> QueryAsync(ProductForecastingQuery query, CancellationToken cancellationToken);
    ValueTask<ProductForecastingMutation> SaveAsync(ProductForecastingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProductForecastingEventSink { ValueTask PublishAsync(ProductForecastingEvent domainEvent, CancellationToken cancellationToken); }