namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LanguageCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record LanguageCatalogRecord(Guid Id, string Name, string Owner, LanguageCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LanguageCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LanguageCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LanguageCatalogQuery(string? SearchText, LanguageCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LanguageCatalogPage(IReadOnlyList<LanguageCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LanguageCatalogMutation(bool Succeeded, string Code, string Message, LanguageCatalogRecord? Record, LanguageCatalogEvent? Event);
public interface ILanguageCatalogRepository
{
    ValueTask<LanguageCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LanguageCatalogPage> QueryAsync(LanguageCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<LanguageCatalogMutation> SaveAsync(LanguageCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILanguageCatalogEventSink { ValueTask PublishAsync(LanguageCatalogEvent domainEvent, CancellationToken cancellationToken); }