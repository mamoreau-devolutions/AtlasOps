namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum NoticeCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record NoticeCatalogRecord(Guid Id, string Name, string Owner, NoticeCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record NoticeCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record NoticeCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record NoticeCatalogQuery(string? SearchText, NoticeCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record NoticeCatalogPage(IReadOnlyList<NoticeCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record NoticeCatalogMutation(bool Succeeded, string Code, string Message, NoticeCatalogRecord? Record, NoticeCatalogEvent? Event);
public interface INoticeCatalogRepository
{
    ValueTask<NoticeCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<NoticeCatalogPage> QueryAsync(NoticeCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<NoticeCatalogMutation> SaveAsync(NoticeCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface INoticeCatalogEventSink { ValueTask PublishAsync(NoticeCatalogEvent domainEvent, CancellationToken cancellationToken); }