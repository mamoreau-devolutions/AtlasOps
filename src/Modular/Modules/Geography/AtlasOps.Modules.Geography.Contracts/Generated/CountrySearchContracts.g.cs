namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CountrySearchState { Draft, Active, Paused, Completed, Archived }
public sealed record CountrySearchRecord(Guid Id, string Name, string Owner, CountrySearchState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CountrySearchCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CountrySearchEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CountrySearchQuery(string? SearchText, CountrySearchState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CountrySearchPage(IReadOnlyList<CountrySearchRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CountrySearchMutation(bool Succeeded, string Code, string Message, CountrySearchRecord? Record, CountrySearchEvent? Event);
public interface ICountrySearchRepository
{
    ValueTask<CountrySearchRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CountrySearchPage> QueryAsync(CountrySearchQuery query, CancellationToken cancellationToken);
    ValueTask<CountrySearchMutation> SaveAsync(CountrySearchRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICountrySearchEventSink { ValueTask PublishAsync(CountrySearchEvent domainEvent, CancellationToken cancellationToken); }