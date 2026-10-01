namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RegionCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record RegionCatalogRecord(Guid Id, string Name, string Owner, RegionCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RegionCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RegionCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RegionCatalogQuery(string? SearchText, RegionCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RegionCatalogPage(IReadOnlyList<RegionCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RegionCatalogMutation(bool Succeeded, string Code, string Message, RegionCatalogRecord? Record, RegionCatalogEvent? Event);
public interface IRegionCatalogRepository
{
    ValueTask<RegionCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RegionCatalogPage> QueryAsync(RegionCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<RegionCatalogMutation> SaveAsync(RegionCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRegionCatalogEventSink { ValueTask PublishAsync(RegionCatalogEvent domainEvent, CancellationToken cancellationToken); }