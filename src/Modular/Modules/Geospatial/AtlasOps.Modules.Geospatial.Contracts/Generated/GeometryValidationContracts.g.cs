namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum GeometryValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record GeometryValidationRecord(Guid Id, string Name, string Owner, GeometryValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record GeometryValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record GeometryValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record GeometryValidationQuery(string? SearchText, GeometryValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record GeometryValidationPage(IReadOnlyList<GeometryValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record GeometryValidationMutation(bool Succeeded, string Code, string Message, GeometryValidationRecord? Record, GeometryValidationEvent? Event);
public interface IGeometryValidationRepository
{
    ValueTask<GeometryValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<GeometryValidationPage> QueryAsync(GeometryValidationQuery query, CancellationToken cancellationToken);
    ValueTask<GeometryValidationMutation> SaveAsync(GeometryValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IGeometryValidationEventSink { ValueTask PublishAsync(GeometryValidationEvent domainEvent, CancellationToken cancellationToken); }