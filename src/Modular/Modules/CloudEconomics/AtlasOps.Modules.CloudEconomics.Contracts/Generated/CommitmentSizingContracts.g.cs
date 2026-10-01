namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CommitmentSizingState { Draft, Active, Paused, Completed, Archived }
public sealed record CommitmentSizingRecord(Guid Id, string Name, string Owner, CommitmentSizingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CommitmentSizingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CommitmentSizingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CommitmentSizingQuery(string? SearchText, CommitmentSizingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CommitmentSizingPage(IReadOnlyList<CommitmentSizingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CommitmentSizingMutation(bool Succeeded, string Code, string Message, CommitmentSizingRecord? Record, CommitmentSizingEvent? Event);
public interface ICommitmentSizingRepository
{
    ValueTask<CommitmentSizingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CommitmentSizingPage> QueryAsync(CommitmentSizingQuery query, CancellationToken cancellationToken);
    ValueTask<CommitmentSizingMutation> SaveAsync(CommitmentSizingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICommitmentSizingEventSink { ValueTask PublishAsync(CommitmentSizingEvent domainEvent, CancellationToken cancellationToken); }