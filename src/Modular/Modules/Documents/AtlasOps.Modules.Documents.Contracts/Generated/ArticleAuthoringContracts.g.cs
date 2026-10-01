namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ArticleAuthoringState { Draft, Active, Paused, Completed, Archived }
public sealed record ArticleAuthoringRecord(Guid Id, string Name, string Owner, ArticleAuthoringState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ArticleAuthoringCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ArticleAuthoringEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ArticleAuthoringQuery(string? SearchText, ArticleAuthoringState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ArticleAuthoringPage(IReadOnlyList<ArticleAuthoringRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ArticleAuthoringMutation(bool Succeeded, string Code, string Message, ArticleAuthoringRecord? Record, ArticleAuthoringEvent? Event);
public interface IArticleAuthoringRepository
{
    ValueTask<ArticleAuthoringRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ArticleAuthoringPage> QueryAsync(ArticleAuthoringQuery query, CancellationToken cancellationToken);
    ValueTask<ArticleAuthoringMutation> SaveAsync(ArticleAuthoringRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IArticleAuthoringEventSink { ValueTask PublishAsync(ArticleAuthoringEvent domainEvent, CancellationToken cancellationToken); }