namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ArticleArchivalState { Draft, Active, Paused, Completed, Archived }
public sealed record ArticleArchivalRecord(Guid Id, string Name, string Owner, ArticleArchivalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ArticleArchivalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ArticleArchivalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ArticleArchivalQuery(string? SearchText, ArticleArchivalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ArticleArchivalPage(IReadOnlyList<ArticleArchivalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ArticleArchivalMutation(bool Succeeded, string Code, string Message, ArticleArchivalRecord? Record, ArticleArchivalEvent? Event);
public interface IArticleArchivalRepository
{
    ValueTask<ArticleArchivalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ArticleArchivalPage> QueryAsync(ArticleArchivalQuery query, CancellationToken cancellationToken);
    ValueTask<ArticleArchivalMutation> SaveAsync(ArticleArchivalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IArticleArchivalEventSink { ValueTask PublishAsync(ArticleArchivalEvent domainEvent, CancellationToken cancellationToken); }