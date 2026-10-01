namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ApprovalCancellationState { Draft, Active, Paused, Completed, Archived }
public sealed record ApprovalCancellationRecord(Guid Id, string Name, string Owner, ApprovalCancellationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ApprovalCancellationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ApprovalCancellationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ApprovalCancellationQuery(string? SearchText, ApprovalCancellationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ApprovalCancellationPage(IReadOnlyList<ApprovalCancellationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ApprovalCancellationMutation(bool Succeeded, string Code, string Message, ApprovalCancellationRecord? Record, ApprovalCancellationEvent? Event);
public interface IApprovalCancellationRepository
{
    ValueTask<ApprovalCancellationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ApprovalCancellationPage> QueryAsync(ApprovalCancellationQuery query, CancellationToken cancellationToken);
    ValueTask<ApprovalCancellationMutation> SaveAsync(ApprovalCancellationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IApprovalCancellationEventSink { ValueTask PublishAsync(ApprovalCancellationEvent domainEvent, CancellationToken cancellationToken); }