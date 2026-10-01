namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ApprovalAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record ApprovalAuditRecord(Guid Id, string Name, string Owner, ApprovalAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ApprovalAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ApprovalAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ApprovalAuditQuery(string? SearchText, ApprovalAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ApprovalAuditPage(IReadOnlyList<ApprovalAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ApprovalAuditMutation(bool Succeeded, string Code, string Message, ApprovalAuditRecord? Record, ApprovalAuditEvent? Event);
public interface IApprovalAuditRepository
{
    ValueTask<ApprovalAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ApprovalAuditPage> QueryAsync(ApprovalAuditQuery query, CancellationToken cancellationToken);
    ValueTask<ApprovalAuditMutation> SaveAsync(ApprovalAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IApprovalAuditEventSink { ValueTask PublishAsync(ApprovalAuditEvent domainEvent, CancellationToken cancellationToken); }