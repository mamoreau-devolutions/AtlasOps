namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum VersionForecastingState { Draft, Active, Paused, Completed, Archived }
public sealed record VersionForecastingRecord(Guid Id, string Name, string Owner, VersionForecastingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record VersionForecastingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record VersionForecastingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record VersionForecastingQuery(string? SearchText, VersionForecastingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record VersionForecastingPage(IReadOnlyList<VersionForecastingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record VersionForecastingMutation(bool Succeeded, string Code, string Message, VersionForecastingRecord? Record, VersionForecastingEvent? Event);
public interface IVersionForecastingRepository
{
    ValueTask<VersionForecastingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<VersionForecastingPage> QueryAsync(VersionForecastingQuery query, CancellationToken cancellationToken);
    ValueTask<VersionForecastingMutation> SaveAsync(VersionForecastingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IVersionForecastingEventSink { ValueTask PublishAsync(VersionForecastingEvent domainEvent, CancellationToken cancellationToken); }