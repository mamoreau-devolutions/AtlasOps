namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LinkPublishingState { Draft, Active, Paused, Completed, Archived }
public sealed record LinkPublishingRecord(Guid Id, string Name, string Owner, LinkPublishingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LinkPublishingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LinkPublishingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LinkPublishingQuery(string? SearchText, LinkPublishingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LinkPublishingPage(IReadOnlyList<LinkPublishingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LinkPublishingMutation(bool Succeeded, string Code, string Message, LinkPublishingRecord? Record, LinkPublishingEvent? Event);
public interface ILinkPublishingRepository
{
    ValueTask<LinkPublishingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LinkPublishingPage> QueryAsync(LinkPublishingQuery query, CancellationToken cancellationToken);
    ValueTask<LinkPublishingMutation> SaveAsync(LinkPublishingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILinkPublishingEventSink { ValueTask PublishAsync(LinkPublishingEvent domainEvent, CancellationToken cancellationToken); }