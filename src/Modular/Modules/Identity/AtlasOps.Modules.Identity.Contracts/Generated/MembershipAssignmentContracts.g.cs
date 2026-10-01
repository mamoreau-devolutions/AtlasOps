namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum MembershipAssignmentState { Draft, Active, Paused, Completed, Archived }
public sealed record MembershipAssignmentRecord(Guid Id, string Name, string Owner, MembershipAssignmentState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record MembershipAssignmentCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record MembershipAssignmentEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record MembershipAssignmentQuery(string? SearchText, MembershipAssignmentState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record MembershipAssignmentPage(IReadOnlyList<MembershipAssignmentRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record MembershipAssignmentMutation(bool Succeeded, string Code, string Message, MembershipAssignmentRecord? Record, MembershipAssignmentEvent? Event);
public interface IMembershipAssignmentRepository
{
    ValueTask<MembershipAssignmentRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<MembershipAssignmentPage> QueryAsync(MembershipAssignmentQuery query, CancellationToken cancellationToken);
    ValueTask<MembershipAssignmentMutation> SaveAsync(MembershipAssignmentRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IMembershipAssignmentEventSink { ValueTask PublishAsync(MembershipAssignmentEvent domainEvent, CancellationToken cancellationToken); }