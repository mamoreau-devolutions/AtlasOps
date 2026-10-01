namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum GeometryIndexingState { Draft, Active, Paused, Completed, Archived }
public sealed record GeometryIndexingRecord(Guid Id, string Name, string Owner, GeometryIndexingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record GeometryIndexingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record GeometryIndexingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record GeometryIndexingQuery(string? SearchText, GeometryIndexingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record GeometryIndexingPage(IReadOnlyList<GeometryIndexingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record GeometryIndexingMutation(bool Succeeded, string Code, string Message, GeometryIndexingRecord? Record, GeometryIndexingEvent? Event);
public interface IGeometryIndexingRepository
{
    ValueTask<GeometryIndexingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<GeometryIndexingPage> QueryAsync(GeometryIndexingQuery query, CancellationToken cancellationToken);
    ValueTask<GeometryIndexingMutation> SaveAsync(GeometryIndexingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IGeometryIndexingEventSink { ValueTask PublishAsync(GeometryIndexingEvent domainEvent, CancellationToken cancellationToken); }