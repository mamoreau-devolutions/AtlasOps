namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CommitmentOptimizationState { Draft, Active, Paused, Completed, Archived }
public sealed record CommitmentOptimizationRecord(Guid Id, string Name, string Owner, CommitmentOptimizationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CommitmentOptimizationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CommitmentOptimizationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CommitmentOptimizationQuery(string? SearchText, CommitmentOptimizationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CommitmentOptimizationPage(IReadOnlyList<CommitmentOptimizationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CommitmentOptimizationMutation(bool Succeeded, string Code, string Message, CommitmentOptimizationRecord? Record, CommitmentOptimizationEvent? Event);
public interface ICommitmentOptimizationRepository
{
    ValueTask<CommitmentOptimizationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CommitmentOptimizationPage> QueryAsync(CommitmentOptimizationQuery query, CancellationToken cancellationToken);
    ValueTask<CommitmentOptimizationMutation> SaveAsync(CommitmentOptimizationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICommitmentOptimizationEventSink { ValueTask PublishAsync(CommitmentOptimizationEvent domainEvent, CancellationToken cancellationToken); }