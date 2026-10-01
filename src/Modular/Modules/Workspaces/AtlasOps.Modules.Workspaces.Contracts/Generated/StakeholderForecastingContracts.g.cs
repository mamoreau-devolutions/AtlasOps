namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum StakeholderForecastingState { Draft, Active, Paused, Completed, Archived }
public sealed record StakeholderForecastingRecord(Guid Id, string Name, string Owner, StakeholderForecastingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record StakeholderForecastingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record StakeholderForecastingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record StakeholderForecastingQuery(string? SearchText, StakeholderForecastingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record StakeholderForecastingPage(IReadOnlyList<StakeholderForecastingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record StakeholderForecastingMutation(bool Succeeded, string Code, string Message, StakeholderForecastingRecord? Record, StakeholderForecastingEvent? Event);
public interface IStakeholderForecastingRepository
{
    ValueTask<StakeholderForecastingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<StakeholderForecastingPage> QueryAsync(StakeholderForecastingQuery query, CancellationToken cancellationToken);
    ValueTask<StakeholderForecastingMutation> SaveAsync(StakeholderForecastingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IStakeholderForecastingEventSink { ValueTask PublishAsync(StakeholderForecastingEvent domainEvent, CancellationToken cancellationToken); }