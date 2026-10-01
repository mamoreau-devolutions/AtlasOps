namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ArticleIndexingState { Draft, Active, Paused, Completed, Archived }
public sealed record ArticleIndexingRecord(Guid Id, string Name, string Owner, ArticleIndexingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ArticleIndexingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ArticleIndexingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ArticleIndexingQuery(string? SearchText, ArticleIndexingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ArticleIndexingPage(IReadOnlyList<ArticleIndexingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ArticleIndexingMutation(bool Succeeded, string Code, string Message, ArticleIndexingRecord? Record, ArticleIndexingEvent? Event);
public interface IArticleIndexingRepository
{
    ValueTask<ArticleIndexingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ArticleIndexingPage> QueryAsync(ArticleIndexingQuery query, CancellationToken cancellationToken);
    ValueTask<ArticleIndexingMutation> SaveAsync(ArticleIndexingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IArticleIndexingEventSink { ValueTask PublishAsync(ArticleIndexingEvent domainEvent, CancellationToken cancellationToken); }