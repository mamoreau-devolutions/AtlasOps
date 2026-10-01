namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum FeatureIntersectionState { Draft, Active, Paused, Completed, Archived }
public sealed record FeatureIntersectionRecord(Guid Id, string Name, string Owner, FeatureIntersectionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record FeatureIntersectionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record FeatureIntersectionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record FeatureIntersectionQuery(string? SearchText, FeatureIntersectionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record FeatureIntersectionPage(IReadOnlyList<FeatureIntersectionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record FeatureIntersectionMutation(bool Succeeded, string Code, string Message, FeatureIntersectionRecord? Record, FeatureIntersectionEvent? Event);
public interface IFeatureIntersectionRepository
{
    ValueTask<FeatureIntersectionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<FeatureIntersectionPage> QueryAsync(FeatureIntersectionQuery query, CancellationToken cancellationToken);
    ValueTask<FeatureIntersectionMutation> SaveAsync(FeatureIntersectionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IFeatureIntersectionEventSink { ValueTask PublishAsync(FeatureIntersectionEvent domainEvent, CancellationToken cancellationToken); }