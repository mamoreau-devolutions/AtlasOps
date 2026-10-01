namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RegionOptimizationState { Draft, Active, Paused, Completed, Archived }
public sealed record RegionOptimizationRecord(Guid Id, string Name, string Owner, RegionOptimizationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RegionOptimizationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RegionOptimizationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RegionOptimizationQuery(string? SearchText, RegionOptimizationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RegionOptimizationPage(IReadOnlyList<RegionOptimizationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RegionOptimizationMutation(bool Succeeded, string Code, string Message, RegionOptimizationRecord? Record, RegionOptimizationEvent? Event);
public interface IRegionOptimizationRepository
{
    ValueTask<RegionOptimizationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RegionOptimizationPage> QueryAsync(RegionOptimizationQuery query, CancellationToken cancellationToken);
    ValueTask<RegionOptimizationMutation> SaveAsync(RegionOptimizationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRegionOptimizationEventSink { ValueTask PublishAsync(RegionOptimizationEvent domainEvent, CancellationToken cancellationToken); }