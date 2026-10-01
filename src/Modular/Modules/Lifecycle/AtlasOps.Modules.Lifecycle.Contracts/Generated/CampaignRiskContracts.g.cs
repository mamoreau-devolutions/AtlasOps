namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CampaignRiskState { Draft, Active, Paused, Completed, Archived }
public sealed record CampaignRiskRecord(Guid Id, string Name, string Owner, CampaignRiskState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CampaignRiskCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CampaignRiskEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CampaignRiskQuery(string? SearchText, CampaignRiskState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CampaignRiskPage(IReadOnlyList<CampaignRiskRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CampaignRiskMutation(bool Succeeded, string Code, string Message, CampaignRiskRecord? Record, CampaignRiskEvent? Event);
public interface ICampaignRiskRepository
{
    ValueTask<CampaignRiskRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CampaignRiskPage> QueryAsync(CampaignRiskQuery query, CancellationToken cancellationToken);
    ValueTask<CampaignRiskMutation> SaveAsync(CampaignRiskRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICampaignRiskEventSink { ValueTask PublishAsync(CampaignRiskEvent domainEvent, CancellationToken cancellationToken); }