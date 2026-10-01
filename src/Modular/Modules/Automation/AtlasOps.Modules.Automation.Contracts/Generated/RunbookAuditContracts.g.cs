namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RunbookAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record RunbookAuditRecord(Guid Id, string Name, string Owner, RunbookAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RunbookAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RunbookAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RunbookAuditQuery(string? SearchText, RunbookAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RunbookAuditPage(IReadOnlyList<RunbookAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RunbookAuditMutation(bool Succeeded, string Code, string Message, RunbookAuditRecord? Record, RunbookAuditEvent? Event);
public interface IRunbookAuditRepository
{
    ValueTask<RunbookAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RunbookAuditPage> QueryAsync(RunbookAuditQuery query, CancellationToken cancellationToken);
    ValueTask<RunbookAuditMutation> SaveAsync(RunbookAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRunbookAuditEventSink { ValueTask PublishAsync(RunbookAuditEvent domainEvent, CancellationToken cancellationToken); }