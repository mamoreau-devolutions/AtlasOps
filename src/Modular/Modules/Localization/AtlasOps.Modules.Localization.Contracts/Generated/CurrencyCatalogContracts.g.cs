namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CurrencyCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record CurrencyCatalogRecord(Guid Id, string Name, string Owner, CurrencyCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CurrencyCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CurrencyCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CurrencyCatalogQuery(string? SearchText, CurrencyCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CurrencyCatalogPage(IReadOnlyList<CurrencyCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CurrencyCatalogMutation(bool Succeeded, string Code, string Message, CurrencyCatalogRecord? Record, CurrencyCatalogEvent? Event);
public interface ICurrencyCatalogRepository
{
    ValueTask<CurrencyCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CurrencyCatalogPage> QueryAsync(CurrencyCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<CurrencyCatalogMutation> SaveAsync(CurrencyCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICurrencyCatalogEventSink { ValueTask PublishAsync(CurrencyCatalogEvent domainEvent, CancellationToken cancellationToken); }