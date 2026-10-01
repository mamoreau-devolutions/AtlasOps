namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ArticleVersioningState { Draft, Active, Paused, Completed, Archived }
public sealed record ArticleVersioningRecord(Guid Id, string Name, string Owner, ArticleVersioningState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ArticleVersioningCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ArticleVersioningEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ArticleVersioningQuery(string? SearchText, ArticleVersioningState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ArticleVersioningPage(IReadOnlyList<ArticleVersioningRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ArticleVersioningMutation(bool Succeeded, string Code, string Message, ArticleVersioningRecord? Record, ArticleVersioningEvent? Event);
public interface IArticleVersioningRepository
{
    ValueTask<ArticleVersioningRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ArticleVersioningPage> QueryAsync(ArticleVersioningQuery query, CancellationToken cancellationToken);
    ValueTask<ArticleVersioningMutation> SaveAsync(ArticleVersioningRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IArticleVersioningEventSink { ValueTask PublishAsync(ArticleVersioningEvent domainEvent, CancellationToken cancellationToken); }