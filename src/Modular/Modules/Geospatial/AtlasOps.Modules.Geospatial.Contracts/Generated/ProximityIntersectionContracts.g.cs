namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProximityIntersectionState { Draft, Active, Paused, Completed, Archived }
public sealed record ProximityIntersectionRecord(Guid Id, string Name, string Owner, ProximityIntersectionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProximityIntersectionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProximityIntersectionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProximityIntersectionQuery(string? SearchText, ProximityIntersectionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProximityIntersectionPage(IReadOnlyList<ProximityIntersectionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProximityIntersectionMutation(bool Succeeded, string Code, string Message, ProximityIntersectionRecord? Record, ProximityIntersectionEvent? Event);
public interface IProximityIntersectionRepository
{
    ValueTask<ProximityIntersectionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProximityIntersectionPage> QueryAsync(ProximityIntersectionQuery query, CancellationToken cancellationToken);
    ValueTask<ProximityIntersectionMutation> SaveAsync(ProximityIntersectionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProximityIntersectionEventSink { ValueTask PublishAsync(ProximityIntersectionEvent domainEvent, CancellationToken cancellationToken); }