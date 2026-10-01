namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RegionNormalizationState { Draft, Active, Paused, Completed, Archived }
public sealed record RegionNormalizationRecord(Guid Id, string Name, string Owner, RegionNormalizationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RegionNormalizationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RegionNormalizationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RegionNormalizationQuery(string? SearchText, RegionNormalizationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RegionNormalizationPage(IReadOnlyList<RegionNormalizationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RegionNormalizationMutation(bool Succeeded, string Code, string Message, RegionNormalizationRecord? Record, RegionNormalizationEvent? Event);
public interface IRegionNormalizationRepository
{
    ValueTask<RegionNormalizationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RegionNormalizationPage> QueryAsync(RegionNormalizationQuery query, CancellationToken cancellationToken);
    ValueTask<RegionNormalizationMutation> SaveAsync(RegionNormalizationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRegionNormalizationEventSink { ValueTask PublishAsync(RegionNormalizationEvent domainEvent, CancellationToken cancellationToken); }