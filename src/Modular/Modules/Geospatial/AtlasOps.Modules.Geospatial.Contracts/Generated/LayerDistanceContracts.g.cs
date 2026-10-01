namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LayerDistanceState { Draft, Active, Paused, Completed, Archived }
public sealed record LayerDistanceRecord(Guid Id, string Name, string Owner, LayerDistanceState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LayerDistanceCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LayerDistanceEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LayerDistanceQuery(string? SearchText, LayerDistanceState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LayerDistancePage(IReadOnlyList<LayerDistanceRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LayerDistanceMutation(bool Succeeded, string Code, string Message, LayerDistanceRecord? Record, LayerDistanceEvent? Event);
public interface ILayerDistanceRepository
{
    ValueTask<LayerDistanceRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LayerDistancePage> QueryAsync(LayerDistanceQuery query, CancellationToken cancellationToken);
    ValueTask<LayerDistanceMutation> SaveAsync(LayerDistanceRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILayerDistanceEventSink { ValueTask PublishAsync(LayerDistanceEvent domainEvent, CancellationToken cancellationToken); }