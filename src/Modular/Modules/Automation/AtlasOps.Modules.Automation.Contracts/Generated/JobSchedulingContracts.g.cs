namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum JobSchedulingState { Draft, Active, Paused, Completed, Archived }
public sealed record JobSchedulingRecord(Guid Id, string Name, string Owner, JobSchedulingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record JobSchedulingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record JobSchedulingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record JobSchedulingQuery(string? SearchText, JobSchedulingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record JobSchedulingPage(IReadOnlyList<JobSchedulingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record JobSchedulingMutation(bool Succeeded, string Code, string Message, JobSchedulingRecord? Record, JobSchedulingEvent? Event);
public interface IJobSchedulingRepository
{
    ValueTask<JobSchedulingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<JobSchedulingPage> QueryAsync(JobSchedulingQuery query, CancellationToken cancellationToken);
    ValueTask<JobSchedulingMutation> SaveAsync(JobSchedulingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IJobSchedulingEventSink { ValueTask PublishAsync(JobSchedulingEvent domainEvent, CancellationToken cancellationToken); }