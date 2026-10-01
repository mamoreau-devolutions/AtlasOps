namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum OfferingOptimizationState { Draft, Active, Paused, Completed, Archived }
public sealed record OfferingOptimizationRecord(Guid Id, string Name, string Owner, OfferingOptimizationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record OfferingOptimizationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record OfferingOptimizationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record OfferingOptimizationQuery(string? SearchText, OfferingOptimizationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record OfferingOptimizationPage(IReadOnlyList<OfferingOptimizationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record OfferingOptimizationMutation(bool Succeeded, string Code, string Message, OfferingOptimizationRecord? Record, OfferingOptimizationEvent? Event);
public interface IOfferingOptimizationRepository
{
    ValueTask<OfferingOptimizationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<OfferingOptimizationPage> QueryAsync(OfferingOptimizationQuery query, CancellationToken cancellationToken);
    ValueTask<OfferingOptimizationMutation> SaveAsync(OfferingOptimizationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IOfferingOptimizationEventSink { ValueTask PublishAsync(OfferingOptimizationEvent domainEvent, CancellationToken cancellationToken); }