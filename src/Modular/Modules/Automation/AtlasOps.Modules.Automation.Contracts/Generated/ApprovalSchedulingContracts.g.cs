namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ApprovalSchedulingState { Draft, Active, Paused, Completed, Archived }
public sealed record ApprovalSchedulingRecord(Guid Id, string Name, string Owner, ApprovalSchedulingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ApprovalSchedulingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ApprovalSchedulingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ApprovalSchedulingQuery(string? SearchText, ApprovalSchedulingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ApprovalSchedulingPage(IReadOnlyList<ApprovalSchedulingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ApprovalSchedulingMutation(bool Succeeded, string Code, string Message, ApprovalSchedulingRecord? Record, ApprovalSchedulingEvent? Event);
public interface IApprovalSchedulingRepository
{
    ValueTask<ApprovalSchedulingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ApprovalSchedulingPage> QueryAsync(ApprovalSchedulingQuery query, CancellationToken cancellationToken);
    ValueTask<ApprovalSchedulingMutation> SaveAsync(ApprovalSchedulingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IApprovalSchedulingEventSink { ValueTask PublishAsync(ApprovalSchedulingEvent domainEvent, CancellationToken cancellationToken); }