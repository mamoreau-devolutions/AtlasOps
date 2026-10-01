namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CommitmentNormalizationState { Draft, Active, Paused, Completed, Archived }
public sealed record CommitmentNormalizationRecord(Guid Id, string Name, string Owner, CommitmentNormalizationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CommitmentNormalizationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CommitmentNormalizationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CommitmentNormalizationQuery(string? SearchText, CommitmentNormalizationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CommitmentNormalizationPage(IReadOnlyList<CommitmentNormalizationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CommitmentNormalizationMutation(bool Succeeded, string Code, string Message, CommitmentNormalizationRecord? Record, CommitmentNormalizationEvent? Event);
public interface ICommitmentNormalizationRepository
{
    ValueTask<CommitmentNormalizationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CommitmentNormalizationPage> QueryAsync(CommitmentNormalizationQuery query, CancellationToken cancellationToken);
    ValueTask<CommitmentNormalizationMutation> SaveAsync(CommitmentNormalizationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICommitmentNormalizationEventSink { ValueTask PublishAsync(CommitmentNormalizationEvent domainEvent, CancellationToken cancellationToken); }