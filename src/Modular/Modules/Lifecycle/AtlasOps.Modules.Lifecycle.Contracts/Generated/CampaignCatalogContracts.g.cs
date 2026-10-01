namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CampaignCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record CampaignCatalogRecord(Guid Id, string Name, string Owner, CampaignCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CampaignCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CampaignCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CampaignCatalogQuery(string? SearchText, CampaignCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CampaignCatalogPage(IReadOnlyList<CampaignCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CampaignCatalogMutation(bool Succeeded, string Code, string Message, CampaignCatalogRecord? Record, CampaignCatalogEvent? Event);
public interface ICampaignCatalogRepository
{
    ValueTask<CampaignCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CampaignCatalogPage> QueryAsync(CampaignCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<CampaignCatalogMutation> SaveAsync(CampaignCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICampaignCatalogEventSink { ValueTask PublishAsync(CampaignCatalogEvent domainEvent, CancellationToken cancellationToken); }