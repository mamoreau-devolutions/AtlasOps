namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AssignmentPolicyState { Draft, Active, Paused, Completed, Archived }
public sealed record AssignmentPolicyRecord(Guid Id, string Name, string Owner, AssignmentPolicyState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AssignmentPolicyCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AssignmentPolicyEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AssignmentPolicyQuery(string? SearchText, AssignmentPolicyState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AssignmentPolicyPage(IReadOnlyList<AssignmentPolicyRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AssignmentPolicyMutation(bool Succeeded, string Code, string Message, AssignmentPolicyRecord? Record, AssignmentPolicyEvent? Event);
public interface IAssignmentPolicyRepository
{
    ValueTask<AssignmentPolicyRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AssignmentPolicyPage> QueryAsync(AssignmentPolicyQuery query, CancellationToken cancellationToken);
    ValueTask<AssignmentPolicyMutation> SaveAsync(AssignmentPolicyRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAssignmentPolicyEventSink { ValueTask PublishAsync(AssignmentPolicyEvent domainEvent, CancellationToken cancellationToken); }