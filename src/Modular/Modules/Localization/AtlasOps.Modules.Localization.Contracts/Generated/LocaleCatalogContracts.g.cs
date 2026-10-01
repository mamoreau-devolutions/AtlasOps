namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LocaleCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record LocaleCatalogRecord(Guid Id, string Name, string Owner, LocaleCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LocaleCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LocaleCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LocaleCatalogQuery(string? SearchText, LocaleCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LocaleCatalogPage(IReadOnlyList<LocaleCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LocaleCatalogMutation(bool Succeeded, string Code, string Message, LocaleCatalogRecord? Record, LocaleCatalogEvent? Event);
public interface ILocaleCatalogRepository
{
    ValueTask<LocaleCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LocaleCatalogPage> QueryAsync(LocaleCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<LocaleCatalogMutation> SaveAsync(LocaleCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILocaleCatalogEventSink { ValueTask PublishAsync(LocaleCatalogEvent domainEvent, CancellationToken cancellationToken); }