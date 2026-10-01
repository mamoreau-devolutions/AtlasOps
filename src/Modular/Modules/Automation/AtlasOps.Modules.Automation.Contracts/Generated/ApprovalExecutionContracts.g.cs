namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ApprovalExecutionState { Draft, Active, Paused, Completed, Archived }
public sealed record ApprovalExecutionRecord(Guid Id, string Name, string Owner, ApprovalExecutionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ApprovalExecutionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ApprovalExecutionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ApprovalExecutionQuery(string? SearchText, ApprovalExecutionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ApprovalExecutionPage(IReadOnlyList<ApprovalExecutionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ApprovalExecutionMutation(bool Succeeded, string Code, string Message, ApprovalExecutionRecord? Record, ApprovalExecutionEvent? Event);
public interface IApprovalExecutionRepository
{
    ValueTask<ApprovalExecutionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ApprovalExecutionPage> QueryAsync(ApprovalExecutionQuery query, CancellationToken cancellationToken);
    ValueTask<ApprovalExecutionMutation> SaveAsync(ApprovalExecutionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IApprovalExecutionEventSink { ValueTask PublishAsync(ApprovalExecutionEvent domainEvent, CancellationToken cancellationToken); }