namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LocationOverrideState { Draft, Active, Paused, Completed, Archived }
public sealed record LocationOverrideRecord(Guid Id, string Name, string Owner, LocationOverrideState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LocationOverrideCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LocationOverrideEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LocationOverrideQuery(string? SearchText, LocationOverrideState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LocationOverridePage(IReadOnlyList<LocationOverrideRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LocationOverrideMutation(bool Succeeded, string Code, string Message, LocationOverrideRecord? Record, LocationOverrideEvent? Event);
public interface ILocationOverrideRepository
{
    ValueTask<LocationOverrideRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LocationOverridePage> QueryAsync(LocationOverrideQuery query, CancellationToken cancellationToken);
    ValueTask<LocationOverrideMutation> SaveAsync(LocationOverrideRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILocationOverrideEventSink { ValueTask PublishAsync(LocationOverrideEvent domainEvent, CancellationToken cancellationToken); }