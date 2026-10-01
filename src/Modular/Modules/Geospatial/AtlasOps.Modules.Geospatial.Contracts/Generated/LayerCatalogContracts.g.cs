namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LayerCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record LayerCatalogRecord(Guid Id, string Name, string Owner, LayerCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LayerCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LayerCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LayerCatalogQuery(string? SearchText, LayerCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LayerCatalogPage(IReadOnlyList<LayerCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LayerCatalogMutation(bool Succeeded, string Code, string Message, LayerCatalogRecord? Record, LayerCatalogEvent? Event);
public interface ILayerCatalogRepository
{
    ValueTask<LayerCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LayerCatalogPage> QueryAsync(LayerCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<LayerCatalogMutation> SaveAsync(LayerCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILayerCatalogEventSink { ValueTask PublishAsync(LayerCatalogEvent domainEvent, CancellationToken cancellationToken); }