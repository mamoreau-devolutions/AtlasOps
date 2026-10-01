namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ExceptionAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record ExceptionAuditRecord(Guid Id, string Name, string Owner, ExceptionAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ExceptionAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ExceptionAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ExceptionAuditQuery(string? SearchText, ExceptionAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ExceptionAuditPage(IReadOnlyList<ExceptionAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ExceptionAuditMutation(bool Succeeded, string Code, string Message, ExceptionAuditRecord? Record, ExceptionAuditEvent? Event);
public interface IExceptionAuditRepository
{
    ValueTask<ExceptionAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ExceptionAuditPage> QueryAsync(ExceptionAuditQuery query, CancellationToken cancellationToken);
    ValueTask<ExceptionAuditMutation> SaveAsync(ExceptionAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IExceptionAuditEventSink { ValueTask PublishAsync(ExceptionAuditEvent domainEvent, CancellationToken cancellationToken); }