namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PolicyApprovalState { Draft, Active, Paused, Completed, Archived }
public sealed record PolicyApprovalRecord(Guid Id, string Name, string Owner, PolicyApprovalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PolicyApprovalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PolicyApprovalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PolicyApprovalQuery(string? SearchText, PolicyApprovalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PolicyApprovalPage(IReadOnlyList<PolicyApprovalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PolicyApprovalMutation(bool Succeeded, string Code, string Message, PolicyApprovalRecord? Record, PolicyApprovalEvent? Event);
public interface IPolicyApprovalRepository
{
    ValueTask<PolicyApprovalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PolicyApprovalPage> QueryAsync(PolicyApprovalQuery query, CancellationToken cancellationToken);
    ValueTask<PolicyApprovalMutation> SaveAsync(PolicyApprovalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPolicyApprovalEventSink { ValueTask PublishAsync(PolicyApprovalEvent domainEvent, CancellationToken cancellationToken); }