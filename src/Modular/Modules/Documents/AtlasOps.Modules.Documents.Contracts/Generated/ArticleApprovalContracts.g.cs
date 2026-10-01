namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ArticleApprovalState { Draft, Active, Paused, Completed, Archived }
public sealed record ArticleApprovalRecord(Guid Id, string Name, string Owner, ArticleApprovalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ArticleApprovalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ArticleApprovalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ArticleApprovalQuery(string? SearchText, ArticleApprovalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ArticleApprovalPage(IReadOnlyList<ArticleApprovalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ArticleApprovalMutation(bool Succeeded, string Code, string Message, ArticleApprovalRecord? Record, ArticleApprovalEvent? Event);
public interface IArticleApprovalRepository
{
    ValueTask<ArticleApprovalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ArticleApprovalPage> QueryAsync(ArticleApprovalQuery query, CancellationToken cancellationToken);
    ValueTask<ArticleApprovalMutation> SaveAsync(ArticleApprovalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IArticleApprovalEventSink { ValueTask PublishAsync(ArticleApprovalEvent domainEvent, CancellationToken cancellationToken); }