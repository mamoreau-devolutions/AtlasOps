namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum IdentityAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record IdentityAuditRecord(Guid Id, string Name, string Owner, IdentityAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record IdentityAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record IdentityAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record IdentityAuditQuery(string? SearchText, IdentityAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record IdentityAuditPage(IReadOnlyList<IdentityAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record IdentityAuditMutation(bool Succeeded, string Code, string Message, IdentityAuditRecord? Record, IdentityAuditEvent? Event);
public interface IIdentityAuditRepository
{
    ValueTask<IdentityAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<IdentityAuditPage> QueryAsync(IdentityAuditQuery query, CancellationToken cancellationToken);
    ValueTask<IdentityAuditMutation> SaveAsync(IdentityAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IIdentityAuditEventSink { ValueTask PublishAsync(IdentityAuditEvent domainEvent, CancellationToken cancellationToken); }