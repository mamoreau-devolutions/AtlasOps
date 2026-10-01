namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CampaignUpgradeState { Draft, Active, Paused, Completed, Archived }
public sealed record CampaignUpgradeRecord(Guid Id, string Name, string Owner, CampaignUpgradeState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CampaignUpgradeCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CampaignUpgradeEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CampaignUpgradeQuery(string? SearchText, CampaignUpgradeState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CampaignUpgradePage(IReadOnlyList<CampaignUpgradeRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CampaignUpgradeMutation(bool Succeeded, string Code, string Message, CampaignUpgradeRecord? Record, CampaignUpgradeEvent? Event);
public interface ICampaignUpgradeRepository
{
    ValueTask<CampaignUpgradeRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CampaignUpgradePage> QueryAsync(CampaignUpgradeQuery query, CancellationToken cancellationToken);
    ValueTask<CampaignUpgradeMutation> SaveAsync(CampaignUpgradeRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICampaignUpgradeEventSink { ValueTask PublishAsync(CampaignUpgradeEvent domainEvent, CancellationToken cancellationToken); }