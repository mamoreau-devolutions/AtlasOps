namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CommitmentReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record CommitmentReportingRecord(Guid Id, string Name, string Owner, CommitmentReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CommitmentReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CommitmentReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CommitmentReportingQuery(string? SearchText, CommitmentReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CommitmentReportingPage(IReadOnlyList<CommitmentReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CommitmentReportingMutation(bool Succeeded, string Code, string Message, CommitmentReportingRecord? Record, CommitmentReportingEvent? Event);
public interface ICommitmentReportingRepository
{
    ValueTask<CommitmentReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CommitmentReportingPage> QueryAsync(CommitmentReportingQuery query, CancellationToken cancellationToken);
    ValueTask<CommitmentReportingMutation> SaveAsync(CommitmentReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICommitmentReportingEventSink { ValueTask PublishAsync(CommitmentReportingEvent domainEvent, CancellationToken cancellationToken); }