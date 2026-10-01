namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LayerIntersectionState { Draft, Active, Paused, Completed, Archived }
public sealed record LayerIntersectionRecord(Guid Id, string Name, string Owner, LayerIntersectionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LayerIntersectionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LayerIntersectionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LayerIntersectionQuery(string? SearchText, LayerIntersectionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LayerIntersectionPage(IReadOnlyList<LayerIntersectionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LayerIntersectionMutation(bool Succeeded, string Code, string Message, LayerIntersectionRecord? Record, LayerIntersectionEvent? Event);
public interface ILayerIntersectionRepository
{
    ValueTask<LayerIntersectionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LayerIntersectionPage> QueryAsync(LayerIntersectionQuery query, CancellationToken cancellationToken);
    ValueTask<LayerIntersectionMutation> SaveAsync(LayerIntersectionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILayerIntersectionEventSink { ValueTask PublishAsync(LayerIntersectionEvent domainEvent, CancellationToken cancellationToken); }