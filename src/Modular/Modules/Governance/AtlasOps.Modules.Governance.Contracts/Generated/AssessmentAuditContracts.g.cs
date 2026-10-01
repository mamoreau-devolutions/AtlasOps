namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AssessmentAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record AssessmentAuditRecord(Guid Id, string Name, string Owner, AssessmentAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AssessmentAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AssessmentAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AssessmentAuditQuery(string? SearchText, AssessmentAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AssessmentAuditPage(IReadOnlyList<AssessmentAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AssessmentAuditMutation(bool Succeeded, string Code, string Message, AssessmentAuditRecord? Record, AssessmentAuditEvent? Event);
public interface IAssessmentAuditRepository
{
    ValueTask<AssessmentAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AssessmentAuditPage> QueryAsync(AssessmentAuditQuery query, CancellationToken cancellationToken);
    ValueTask<AssessmentAuditMutation> SaveAsync(AssessmentAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAssessmentAuditEventSink { ValueTask PublishAsync(AssessmentAuditEvent domainEvent, CancellationToken cancellationToken); }