namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum JobExecutionState { Draft, Active, Paused, Completed, Archived }
public sealed record JobExecutionRecord(Guid Id, string Name, string Owner, JobExecutionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record JobExecutionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record JobExecutionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record JobExecutionQuery(string? SearchText, JobExecutionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record JobExecutionPage(IReadOnlyList<JobExecutionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record JobExecutionMutation(bool Succeeded, string Code, string Message, JobExecutionRecord? Record, JobExecutionEvent? Event);
public interface IJobExecutionRepository
{
    ValueTask<JobExecutionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<JobExecutionPage> QueryAsync(JobExecutionQuery query, CancellationToken cancellationToken);
    ValueTask<JobExecutionMutation> SaveAsync(JobExecutionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IJobExecutionEventSink { ValueTask PublishAsync(JobExecutionEvent domainEvent, CancellationToken cancellationToken); }