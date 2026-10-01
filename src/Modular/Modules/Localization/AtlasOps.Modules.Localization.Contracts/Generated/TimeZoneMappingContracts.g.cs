namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TimeZoneMappingState { Draft, Active, Paused, Completed, Archived }
public sealed record TimeZoneMappingRecord(Guid Id, string Name, string Owner, TimeZoneMappingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TimeZoneMappingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TimeZoneMappingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TimeZoneMappingQuery(string? SearchText, TimeZoneMappingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TimeZoneMappingPage(IReadOnlyList<TimeZoneMappingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TimeZoneMappingMutation(bool Succeeded, string Code, string Message, TimeZoneMappingRecord? Record, TimeZoneMappingEvent? Event);
public interface ITimeZoneMappingRepository
{
    ValueTask<TimeZoneMappingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TimeZoneMappingPage> QueryAsync(TimeZoneMappingQuery query, CancellationToken cancellationToken);
    ValueTask<TimeZoneMappingMutation> SaveAsync(TimeZoneMappingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITimeZoneMappingEventSink { ValueTask PublishAsync(TimeZoneMappingEvent domainEvent, CancellationToken cancellationToken); }