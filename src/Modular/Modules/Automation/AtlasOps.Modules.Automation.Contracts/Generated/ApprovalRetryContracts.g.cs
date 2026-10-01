namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ApprovalRetryState { Draft, Active, Paused, Completed, Archived }
public sealed record ApprovalRetryRecord(Guid Id, string Name, string Owner, ApprovalRetryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ApprovalRetryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ApprovalRetryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ApprovalRetryQuery(string? SearchText, ApprovalRetryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ApprovalRetryPage(IReadOnlyList<ApprovalRetryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ApprovalRetryMutation(bool Succeeded, string Code, string Message, ApprovalRetryRecord? Record, ApprovalRetryEvent? Event);
public interface IApprovalRetryRepository
{
    ValueTask<ApprovalRetryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ApprovalRetryPage> QueryAsync(ApprovalRetryQuery query, CancellationToken cancellationToken);
    ValueTask<ApprovalRetryMutation> SaveAsync(ApprovalRetryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IApprovalRetryEventSink { ValueTask PublishAsync(ApprovalRetryEvent domainEvent, CancellationToken cancellationToken); }