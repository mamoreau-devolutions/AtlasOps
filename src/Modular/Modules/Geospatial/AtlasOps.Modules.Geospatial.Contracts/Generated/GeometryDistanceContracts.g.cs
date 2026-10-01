namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum GeometryDistanceState { Draft, Active, Paused, Completed, Archived }
public sealed record GeometryDistanceRecord(Guid Id, string Name, string Owner, GeometryDistanceState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record GeometryDistanceCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record GeometryDistanceEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record GeometryDistanceQuery(string? SearchText, GeometryDistanceState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record GeometryDistancePage(IReadOnlyList<GeometryDistanceRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record GeometryDistanceMutation(bool Succeeded, string Code, string Message, GeometryDistanceRecord? Record, GeometryDistanceEvent? Event);
public interface IGeometryDistanceRepository
{
    ValueTask<GeometryDistanceRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<GeometryDistancePage> QueryAsync(GeometryDistanceQuery query, CancellationToken cancellationToken);
    ValueTask<GeometryDistanceMutation> SaveAsync(GeometryDistanceRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IGeometryDistanceEventSink { ValueTask PublishAsync(GeometryDistanceEvent domainEvent, CancellationToken cancellationToken); }