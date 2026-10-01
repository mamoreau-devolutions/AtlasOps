namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum FeatureCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record FeatureCatalogRecord(Guid Id, string Name, string Owner, FeatureCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record FeatureCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record FeatureCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record FeatureCatalogQuery(string? SearchText, FeatureCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record FeatureCatalogPage(IReadOnlyList<FeatureCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record FeatureCatalogMutation(bool Succeeded, string Code, string Message, FeatureCatalogRecord? Record, FeatureCatalogEvent? Event);
public interface IFeatureCatalogRepository
{
    ValueTask<FeatureCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<FeatureCatalogPage> QueryAsync(FeatureCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<FeatureCatalogMutation> SaveAsync(FeatureCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IFeatureCatalogEventSink { ValueTask PublishAsync(FeatureCatalogEvent domainEvent, CancellationToken cancellationToken); }