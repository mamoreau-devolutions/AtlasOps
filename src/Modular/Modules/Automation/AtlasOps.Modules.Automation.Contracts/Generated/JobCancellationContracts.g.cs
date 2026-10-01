namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum JobCancellationState { Draft, Active, Paused, Completed, Archived }
public sealed record JobCancellationRecord(Guid Id, string Name, string Owner, JobCancellationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record JobCancellationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record JobCancellationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record JobCancellationQuery(string? SearchText, JobCancellationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record JobCancellationPage(IReadOnlyList<JobCancellationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record JobCancellationMutation(bool Succeeded, string Code, string Message, JobCancellationRecord? Record, JobCancellationEvent? Event);
public interface IJobCancellationRepository
{
    ValueTask<JobCancellationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<JobCancellationPage> QueryAsync(JobCancellationQuery query, CancellationToken cancellationToken);
    ValueTask<JobCancellationMutation> SaveAsync(JobCancellationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IJobCancellationEventSink { ValueTask PublishAsync(JobCancellationEvent domainEvent, CancellationToken cancellationToken); }