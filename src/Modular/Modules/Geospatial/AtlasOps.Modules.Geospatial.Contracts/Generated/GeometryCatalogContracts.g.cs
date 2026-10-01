namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum GeometryCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record GeometryCatalogRecord(Guid Id, string Name, string Owner, GeometryCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record GeometryCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record GeometryCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record GeometryCatalogQuery(string? SearchText, GeometryCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record GeometryCatalogPage(IReadOnlyList<GeometryCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record GeometryCatalogMutation(bool Succeeded, string Code, string Message, GeometryCatalogRecord? Record, GeometryCatalogEvent? Event);
public interface IGeometryCatalogRepository
{
    ValueTask<GeometryCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<GeometryCatalogPage> QueryAsync(GeometryCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<GeometryCatalogMutation> SaveAsync(GeometryCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IGeometryCatalogEventSink { ValueTask PublishAsync(GeometryCatalogEvent domainEvent, CancellationToken cancellationToken); }