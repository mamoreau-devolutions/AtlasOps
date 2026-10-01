namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RouteCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record RouteCatalogRecord(Guid Id, string Name, string Owner, RouteCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RouteCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RouteCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RouteCatalogQuery(string? SearchText, RouteCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RouteCatalogPage(IReadOnlyList<RouteCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RouteCatalogMutation(bool Succeeded, string Code, string Message, RouteCatalogRecord? Record, RouteCatalogEvent? Event);
public interface IRouteCatalogRepository
{
    ValueTask<RouteCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RouteCatalogPage> QueryAsync(RouteCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<RouteCatalogMutation> SaveAsync(RouteCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRouteCatalogEventSink { ValueTask PublishAsync(RouteCatalogEvent domainEvent, CancellationToken cancellationToken); }