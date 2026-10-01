namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PriceOptimizationState { Draft, Active, Paused, Completed, Archived }
public sealed record PriceOptimizationRecord(Guid Id, string Name, string Owner, PriceOptimizationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PriceOptimizationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PriceOptimizationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PriceOptimizationQuery(string? SearchText, PriceOptimizationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PriceOptimizationPage(IReadOnlyList<PriceOptimizationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PriceOptimizationMutation(bool Succeeded, string Code, string Message, PriceOptimizationRecord? Record, PriceOptimizationEvent? Event);
public interface IPriceOptimizationRepository
{
    ValueTask<PriceOptimizationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PriceOptimizationPage> QueryAsync(PriceOptimizationQuery query, CancellationToken cancellationToken);
    ValueTask<PriceOptimizationMutation> SaveAsync(PriceOptimizationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPriceOptimizationEventSink { ValueTask PublishAsync(PriceOptimizationEvent domainEvent, CancellationToken cancellationToken); }