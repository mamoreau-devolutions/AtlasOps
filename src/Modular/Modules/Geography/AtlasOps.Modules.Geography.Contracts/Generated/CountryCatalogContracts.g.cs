namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CountryCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record CountryCatalogRecord(Guid Id, string Name, string Owner, CountryCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CountryCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CountryCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CountryCatalogQuery(string? SearchText, CountryCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CountryCatalogPage(IReadOnlyList<CountryCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CountryCatalogMutation(bool Succeeded, string Code, string Message, CountryCatalogRecord? Record, CountryCatalogEvent? Event);
public interface ICountryCatalogRepository
{
    ValueTask<CountryCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CountryCatalogPage> QueryAsync(CountryCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<CountryCatalogMutation> SaveAsync(CountryCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICountryCatalogEventSink { ValueTask PublishAsync(CountryCatalogEvent domainEvent, CancellationToken cancellationToken); }