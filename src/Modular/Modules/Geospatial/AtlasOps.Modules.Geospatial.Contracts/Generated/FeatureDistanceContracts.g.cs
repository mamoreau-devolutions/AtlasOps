namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum FeatureDistanceState { Draft, Active, Paused, Completed, Archived }
public sealed record FeatureDistanceRecord(Guid Id, string Name, string Owner, FeatureDistanceState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record FeatureDistanceCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record FeatureDistanceEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record FeatureDistanceQuery(string? SearchText, FeatureDistanceState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record FeatureDistancePage(IReadOnlyList<FeatureDistanceRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record FeatureDistanceMutation(bool Succeeded, string Code, string Message, FeatureDistanceRecord? Record, FeatureDistanceEvent? Event);
public interface IFeatureDistanceRepository
{
    ValueTask<FeatureDistanceRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<FeatureDistancePage> QueryAsync(FeatureDistanceQuery query, CancellationToken cancellationToken);
    ValueTask<FeatureDistanceMutation> SaveAsync(FeatureDistanceRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IFeatureDistanceEventSink { ValueTask PublishAsync(FeatureDistanceEvent domainEvent, CancellationToken cancellationToken); }