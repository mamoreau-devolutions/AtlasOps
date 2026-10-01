namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RoleAssignmentState { Draft, Active, Paused, Completed, Archived }
public sealed record RoleAssignmentRecord(Guid Id, string Name, string Owner, RoleAssignmentState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RoleAssignmentCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RoleAssignmentEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RoleAssignmentQuery(string? SearchText, RoleAssignmentState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RoleAssignmentPage(IReadOnlyList<RoleAssignmentRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RoleAssignmentMutation(bool Succeeded, string Code, string Message, RoleAssignmentRecord? Record, RoleAssignmentEvent? Event);
public interface IRoleAssignmentRepository
{
    ValueTask<RoleAssignmentRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RoleAssignmentPage> QueryAsync(RoleAssignmentQuery query, CancellationToken cancellationToken);
    ValueTask<RoleAssignmentMutation> SaveAsync(RoleAssignmentRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRoleAssignmentEventSink { ValueTask PublishAsync(RoleAssignmentEvent domainEvent, CancellationToken cancellationToken); }