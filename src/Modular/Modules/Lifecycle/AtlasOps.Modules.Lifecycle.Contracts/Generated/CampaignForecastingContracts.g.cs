namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CampaignForecastingState { Draft, Active, Paused, Completed, Archived }
public sealed record CampaignForecastingRecord(Guid Id, string Name, string Owner, CampaignForecastingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CampaignForecastingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CampaignForecastingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CampaignForecastingQuery(string? SearchText, CampaignForecastingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CampaignForecastingPage(IReadOnlyList<CampaignForecastingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CampaignForecastingMutation(bool Succeeded, string Code, string Message, CampaignForecastingRecord? Record, CampaignForecastingEvent? Event);
public interface ICampaignForecastingRepository
{
    ValueTask<CampaignForecastingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CampaignForecastingPage> QueryAsync(CampaignForecastingQuery query, CancellationToken cancellationToken);
    ValueTask<CampaignForecastingMutation> SaveAsync(CampaignForecastingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICampaignForecastingEventSink { ValueTask PublishAsync(CampaignForecastingEvent domainEvent, CancellationToken cancellationToken); }