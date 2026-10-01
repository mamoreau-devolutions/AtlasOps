namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProximityDistanceState { Draft, Active, Paused, Completed, Archived }
public sealed record ProximityDistanceRecord(Guid Id, string Name, string Owner, ProximityDistanceState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProximityDistanceCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProximityDistanceEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProximityDistanceQuery(string? SearchText, ProximityDistanceState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProximityDistancePage(IReadOnlyList<ProximityDistanceRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProximityDistanceMutation(bool Succeeded, string Code, string Message, ProximityDistanceRecord? Record, ProximityDistanceEvent? Event);
public interface IProximityDistanceRepository
{
    ValueTask<ProximityDistanceRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProximityDistancePage> QueryAsync(ProximityDistanceQuery query, CancellationToken cancellationToken);
    ValueTask<ProximityDistanceMutation> SaveAsync(ProximityDistanceRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProximityDistanceEventSink { ValueTask PublishAsync(ProximityDistanceEvent domainEvent, CancellationToken cancellationToken); }