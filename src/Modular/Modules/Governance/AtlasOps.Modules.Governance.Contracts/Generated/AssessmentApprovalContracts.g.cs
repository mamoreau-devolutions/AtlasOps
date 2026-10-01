namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AssessmentApprovalState { Draft, Active, Paused, Completed, Archived }
public sealed record AssessmentApprovalRecord(Guid Id, string Name, string Owner, AssessmentApprovalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AssessmentApprovalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AssessmentApprovalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AssessmentApprovalQuery(string? SearchText, AssessmentApprovalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AssessmentApprovalPage(IReadOnlyList<AssessmentApprovalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AssessmentApprovalMutation(bool Succeeded, string Code, string Message, AssessmentApprovalRecord? Record, AssessmentApprovalEvent? Event);
public interface IAssessmentApprovalRepository
{
    ValueTask<AssessmentApprovalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AssessmentApprovalPage> QueryAsync(AssessmentApprovalQuery query, CancellationToken cancellationToken);
    ValueTask<AssessmentApprovalMutation> SaveAsync(AssessmentApprovalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAssessmentApprovalEventSink { ValueTask PublishAsync(AssessmentApprovalEvent domainEvent, CancellationToken cancellationToken); }