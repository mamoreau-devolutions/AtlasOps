namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum FeatureIndexingState { Draft, Active, Paused, Completed, Archived }
public sealed record FeatureIndexingRecord(Guid Id, string Name, string Owner, FeatureIndexingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record FeatureIndexingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record FeatureIndexingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record FeatureIndexingQuery(string? SearchText, FeatureIndexingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record FeatureIndexingPage(IReadOnlyList<FeatureIndexingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record FeatureIndexingMutation(bool Succeeded, string Code, string Message, FeatureIndexingRecord? Record, FeatureIndexingEvent? Event);
public interface IFeatureIndexingRepository
{
    ValueTask<FeatureIndexingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<FeatureIndexingPage> QueryAsync(FeatureIndexingQuery query, CancellationToken cancellationToken);
    ValueTask<FeatureIndexingMutation> SaveAsync(FeatureIndexingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IFeatureIndexingEventSink { ValueTask PublishAsync(FeatureIndexingEvent domainEvent, CancellationToken cancellationToken); }