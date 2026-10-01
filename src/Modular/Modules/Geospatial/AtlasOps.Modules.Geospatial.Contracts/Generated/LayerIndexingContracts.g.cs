namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LayerIndexingState { Draft, Active, Paused, Completed, Archived }
public sealed record LayerIndexingRecord(Guid Id, string Name, string Owner, LayerIndexingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LayerIndexingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LayerIndexingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LayerIndexingQuery(string? SearchText, LayerIndexingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LayerIndexingPage(IReadOnlyList<LayerIndexingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LayerIndexingMutation(bool Succeeded, string Code, string Message, LayerIndexingRecord? Record, LayerIndexingEvent? Event);
public interface ILayerIndexingRepository
{
    ValueTask<LayerIndexingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LayerIndexingPage> QueryAsync(LayerIndexingQuery query, CancellationToken cancellationToken);
    ValueTask<LayerIndexingMutation> SaveAsync(LayerIndexingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILayerIndexingEventSink { ValueTask PublishAsync(LayerIndexingEvent domainEvent, CancellationToken cancellationToken); }