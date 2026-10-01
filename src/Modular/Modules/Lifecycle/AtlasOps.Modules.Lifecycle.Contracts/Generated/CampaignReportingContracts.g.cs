namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CampaignReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record CampaignReportingRecord(Guid Id, string Name, string Owner, CampaignReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CampaignReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CampaignReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CampaignReportingQuery(string? SearchText, CampaignReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CampaignReportingPage(IReadOnlyList<CampaignReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CampaignReportingMutation(bool Succeeded, string Code, string Message, CampaignReportingRecord? Record, CampaignReportingEvent? Event);
public interface ICampaignReportingRepository
{
    ValueTask<CampaignReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CampaignReportingPage> QueryAsync(CampaignReportingQuery query, CancellationToken cancellationToken);
    ValueTask<CampaignReportingMutation> SaveAsync(CampaignReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICampaignReportingEventSink { ValueTask PublishAsync(CampaignReportingEvent domainEvent, CancellationToken cancellationToken); }