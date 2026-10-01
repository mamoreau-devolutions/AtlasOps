namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LocationCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record LocationCatalogRecord(Guid Id, string Name, string Owner, LocationCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LocationCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LocationCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LocationCatalogQuery(string? SearchText, LocationCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LocationCatalogPage(IReadOnlyList<LocationCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LocationCatalogMutation(bool Succeeded, string Code, string Message, LocationCatalogRecord? Record, LocationCatalogEvent? Event);
public interface ILocationCatalogRepository
{
    ValueTask<LocationCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LocationCatalogPage> QueryAsync(LocationCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<LocationCatalogMutation> SaveAsync(LocationCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILocationCatalogEventSink { ValueTask PublishAsync(LocationCatalogEvent domainEvent, CancellationToken cancellationToken); }