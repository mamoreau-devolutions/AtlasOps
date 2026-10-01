namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ArticlePublishingState { Draft, Active, Paused, Completed, Archived }
public sealed record ArticlePublishingRecord(Guid Id, string Name, string Owner, ArticlePublishingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ArticlePublishingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ArticlePublishingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ArticlePublishingQuery(string? SearchText, ArticlePublishingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ArticlePublishingPage(IReadOnlyList<ArticlePublishingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ArticlePublishingMutation(bool Succeeded, string Code, string Message, ArticlePublishingRecord? Record, ArticlePublishingEvent? Event);
public interface IArticlePublishingRepository
{
    ValueTask<ArticlePublishingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ArticlePublishingPage> QueryAsync(ArticlePublishingQuery query, CancellationToken cancellationToken);
    ValueTask<ArticlePublishingMutation> SaveAsync(ArticlePublishingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IArticlePublishingEventSink { ValueTask PublishAsync(ArticlePublishingEvent domainEvent, CancellationToken cancellationToken); }