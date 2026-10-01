namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum JobAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record JobAuditRecord(Guid Id, string Name, string Owner, JobAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record JobAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record JobAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record JobAuditQuery(string? SearchText, JobAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record JobAuditPage(IReadOnlyList<JobAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record JobAuditMutation(bool Succeeded, string Code, string Message, JobAuditRecord? Record, JobAuditEvent? Event);
public interface IJobAuditRepository
{
    ValueTask<JobAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<JobAuditPage> QueryAsync(JobAuditQuery query, CancellationToken cancellationToken);
    ValueTask<JobAuditMutation> SaveAsync(JobAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IJobAuditEventSink { ValueTask PublishAsync(JobAuditEvent domainEvent, CancellationToken cancellationToken); }