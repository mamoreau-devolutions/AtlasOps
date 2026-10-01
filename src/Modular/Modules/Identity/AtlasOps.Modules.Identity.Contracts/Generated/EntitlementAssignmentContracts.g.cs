namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EntitlementAssignmentState { Draft, Active, Paused, Completed, Archived }
public sealed record EntitlementAssignmentRecord(Guid Id, string Name, string Owner, EntitlementAssignmentState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EntitlementAssignmentCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EntitlementAssignmentEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EntitlementAssignmentQuery(string? SearchText, EntitlementAssignmentState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EntitlementAssignmentPage(IReadOnlyList<EntitlementAssignmentRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EntitlementAssignmentMutation(bool Succeeded, string Code, string Message, EntitlementAssignmentRecord? Record, EntitlementAssignmentEvent? Event);
public interface IEntitlementAssignmentRepository
{
    ValueTask<EntitlementAssignmentRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EntitlementAssignmentPage> QueryAsync(EntitlementAssignmentQuery query, CancellationToken cancellationToken);
    ValueTask<EntitlementAssignmentMutation> SaveAsync(EntitlementAssignmentRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEntitlementAssignmentEventSink { ValueTask PublishAsync(EntitlementAssignmentEvent domainEvent, CancellationToken cancellationToken); }