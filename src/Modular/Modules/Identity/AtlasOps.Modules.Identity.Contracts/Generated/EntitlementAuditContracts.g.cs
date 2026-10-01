namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EntitlementAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record EntitlementAuditRecord(Guid Id, string Name, string Owner, EntitlementAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EntitlementAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EntitlementAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EntitlementAuditQuery(string? SearchText, EntitlementAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EntitlementAuditPage(IReadOnlyList<EntitlementAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EntitlementAuditMutation(bool Succeeded, string Code, string Message, EntitlementAuditRecord? Record, EntitlementAuditEvent? Event);
public interface IEntitlementAuditRepository
{
    ValueTask<EntitlementAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EntitlementAuditPage> QueryAsync(EntitlementAuditQuery query, CancellationToken cancellationToken);
    ValueTask<EntitlementAuditMutation> SaveAsync(EntitlementAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEntitlementAuditEventSink { ValueTask PublishAsync(EntitlementAuditEvent domainEvent, CancellationToken cancellationToken); }