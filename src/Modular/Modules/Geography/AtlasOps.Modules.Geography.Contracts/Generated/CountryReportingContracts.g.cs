namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CountryReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record CountryReportingRecord(Guid Id, string Name, string Owner, CountryReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CountryReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CountryReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CountryReportingQuery(string? SearchText, CountryReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CountryReportingPage(IReadOnlyList<CountryReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CountryReportingMutation(bool Succeeded, string Code, string Message, CountryReportingRecord? Record, CountryReportingEvent? Event);
public interface ICountryReportingRepository
{
    ValueTask<CountryReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CountryReportingPage> QueryAsync(CountryReportingQuery query, CancellationToken cancellationToken);
    ValueTask<CountryReportingMutation> SaveAsync(CountryReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICountryReportingEventSink { ValueTask PublishAsync(CountryReportingEvent domainEvent, CancellationToken cancellationToken); }