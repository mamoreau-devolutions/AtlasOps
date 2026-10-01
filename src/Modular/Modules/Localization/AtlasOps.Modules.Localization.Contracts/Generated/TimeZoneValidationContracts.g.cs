namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TimeZoneValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record TimeZoneValidationRecord(Guid Id, string Name, string Owner, TimeZoneValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TimeZoneValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TimeZoneValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TimeZoneValidationQuery(string? SearchText, TimeZoneValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TimeZoneValidationPage(IReadOnlyList<TimeZoneValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TimeZoneValidationMutation(bool Succeeded, string Code, string Message, TimeZoneValidationRecord? Record, TimeZoneValidationEvent? Event);
public interface ITimeZoneValidationRepository
{
    ValueTask<TimeZoneValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TimeZoneValidationPage> QueryAsync(TimeZoneValidationQuery query, CancellationToken cancellationToken);
    ValueTask<TimeZoneValidationMutation> SaveAsync(TimeZoneValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITimeZoneValidationEventSink { ValueTask PublishAsync(TimeZoneValidationEvent domainEvent, CancellationToken cancellationToken); }