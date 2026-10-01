namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CampaignClassificationState { Draft, Active, Paused, Completed, Archived }
public sealed record CampaignClassificationRecord(Guid Id, string Name, string Owner, CampaignClassificationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CampaignClassificationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CampaignClassificationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CampaignClassificationQuery(string? SearchText, CampaignClassificationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CampaignClassificationPage(IReadOnlyList<CampaignClassificationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CampaignClassificationMutation(bool Succeeded, string Code, string Message, CampaignClassificationRecord? Record, CampaignClassificationEvent? Event);
public interface ICampaignClassificationRepository
{
    ValueTask<CampaignClassificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CampaignClassificationPage> QueryAsync(CampaignClassificationQuery query, CancellationToken cancellationToken);
    ValueTask<CampaignClassificationMutation> SaveAsync(CampaignClassificationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICampaignClassificationEventSink { ValueTask PublishAsync(CampaignClassificationEvent domainEvent, CancellationToken cancellationToken); }