namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum IdentityAssignmentState { Draft, Active, Paused, Completed, Archived }
public sealed record IdentityAssignmentRecord(Guid Id, string Name, string Owner, IdentityAssignmentState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record IdentityAssignmentCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record IdentityAssignmentEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record IdentityAssignmentQuery(string? SearchText, IdentityAssignmentState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record IdentityAssignmentPage(IReadOnlyList<IdentityAssignmentRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record IdentityAssignmentMutation(bool Succeeded, string Code, string Message, IdentityAssignmentRecord? Record, IdentityAssignmentEvent? Event);
public interface IIdentityAssignmentRepository
{
    ValueTask<IdentityAssignmentRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<IdentityAssignmentPage> QueryAsync(IdentityAssignmentQuery query, CancellationToken cancellationToken);
    ValueTask<IdentityAssignmentMutation> SaveAsync(IdentityAssignmentRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IIdentityAssignmentEventSink { ValueTask PublishAsync(IdentityAssignmentEvent domainEvent, CancellationToken cancellationToken); }