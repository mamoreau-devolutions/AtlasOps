namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CountryOverrideState { Draft, Active, Paused, Completed, Archived }
public sealed record CountryOverrideRecord(Guid Id, string Name, string Owner, CountryOverrideState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CountryOverrideCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CountryOverrideEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CountryOverrideQuery(string? SearchText, CountryOverrideState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CountryOverridePage(IReadOnlyList<CountryOverrideRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CountryOverrideMutation(bool Succeeded, string Code, string Message, CountryOverrideRecord? Record, CountryOverrideEvent? Event);
public interface ICountryOverrideRepository
{
    ValueTask<CountryOverrideRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CountryOverridePage> QueryAsync(CountryOverrideQuery query, CancellationToken cancellationToken);
    ValueTask<CountryOverrideMutation> SaveAsync(CountryOverrideRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICountryOverrideEventSink { ValueTask PublishAsync(CountryOverrideEvent domainEvent, CancellationToken cancellationToken); }