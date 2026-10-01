namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CountryComparisonState { Draft, Active, Paused, Completed, Archived }
public sealed record CountryComparisonRecord(Guid Id, string Name, string Owner, CountryComparisonState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CountryComparisonCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CountryComparisonEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CountryComparisonQuery(string? SearchText, CountryComparisonState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CountryComparisonPage(IReadOnlyList<CountryComparisonRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CountryComparisonMutation(bool Succeeded, string Code, string Message, CountryComparisonRecord? Record, CountryComparisonEvent? Event);
public interface ICountryComparisonRepository
{
    ValueTask<CountryComparisonRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CountryComparisonPage> QueryAsync(CountryComparisonQuery query, CancellationToken cancellationToken);
    ValueTask<CountryComparisonMutation> SaveAsync(CountryComparisonRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICountryComparisonEventSink { ValueTask PublishAsync(CountryComparisonEvent domainEvent, CancellationToken cancellationToken); }