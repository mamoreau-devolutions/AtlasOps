namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum JobRetryState { Draft, Active, Paused, Completed, Archived }
public sealed record JobRetryRecord(Guid Id, string Name, string Owner, JobRetryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record JobRetryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record JobRetryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record JobRetryQuery(string? SearchText, JobRetryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record JobRetryPage(IReadOnlyList<JobRetryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record JobRetryMutation(bool Succeeded, string Code, string Message, JobRetryRecord? Record, JobRetryEvent? Event);
public interface IJobRetryRepository
{
    ValueTask<JobRetryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<JobRetryPage> QueryAsync(JobRetryQuery query, CancellationToken cancellationToken);
    ValueTask<JobRetryMutation> SaveAsync(JobRetryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IJobRetryEventSink { ValueTask PublishAsync(JobRetryEvent domainEvent, CancellationToken cancellationToken); }