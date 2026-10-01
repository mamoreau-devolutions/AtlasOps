namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProviderOptimizationState { Draft, Active, Paused, Completed, Archived }
public sealed record ProviderOptimizationRecord(Guid Id, string Name, string Owner, ProviderOptimizationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProviderOptimizationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProviderOptimizationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProviderOptimizationQuery(string? SearchText, ProviderOptimizationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProviderOptimizationPage(IReadOnlyList<ProviderOptimizationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProviderOptimizationMutation(bool Succeeded, string Code, string Message, ProviderOptimizationRecord? Record, ProviderOptimizationEvent? Event);
public interface IProviderOptimizationRepository
{
    ValueTask<ProviderOptimizationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProviderOptimizationPage> QueryAsync(ProviderOptimizationQuery query, CancellationToken cancellationToken);
    ValueTask<ProviderOptimizationMutation> SaveAsync(ProviderOptimizationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProviderOptimizationEventSink { ValueTask PublishAsync(ProviderOptimizationEvent domainEvent, CancellationToken cancellationToken); }