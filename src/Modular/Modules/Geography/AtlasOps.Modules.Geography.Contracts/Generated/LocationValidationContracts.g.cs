namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LocationValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record LocationValidationRecord(Guid Id, string Name, string Owner, LocationValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LocationValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LocationValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LocationValidationQuery(string? SearchText, LocationValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LocationValidationPage(IReadOnlyList<LocationValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LocationValidationMutation(bool Succeeded, string Code, string Message, LocationValidationRecord? Record, LocationValidationEvent? Event);
public interface ILocationValidationRepository
{
    ValueTask<LocationValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LocationValidationPage> QueryAsync(LocationValidationQuery query, CancellationToken cancellationToken);
    ValueTask<LocationValidationMutation> SaveAsync(LocationValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILocationValidationEventSink { ValueTask PublishAsync(LocationValidationEvent domainEvent, CancellationToken cancellationToken); }