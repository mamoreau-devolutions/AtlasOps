namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RoleAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record RoleAuditRecord(Guid Id, string Name, string Owner, RoleAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RoleAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RoleAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RoleAuditQuery(string? SearchText, RoleAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RoleAuditPage(IReadOnlyList<RoleAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RoleAuditMutation(bool Succeeded, string Code, string Message, RoleAuditRecord? Record, RoleAuditEvent? Event);
public interface IRoleAuditRepository
{
    ValueTask<RoleAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RoleAuditPage> QueryAsync(RoleAuditQuery query, CancellationToken cancellationToken);
    ValueTask<RoleAuditMutation> SaveAsync(RoleAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRoleAuditEventSink { ValueTask PublishAsync(RoleAuditEvent domainEvent, CancellationToken cancellationToken); }