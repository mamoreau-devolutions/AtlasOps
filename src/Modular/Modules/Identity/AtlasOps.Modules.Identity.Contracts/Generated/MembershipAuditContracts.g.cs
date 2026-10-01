namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum MembershipAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record MembershipAuditRecord(Guid Id, string Name, string Owner, MembershipAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record MembershipAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record MembershipAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record MembershipAuditQuery(string? SearchText, MembershipAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record MembershipAuditPage(IReadOnlyList<MembershipAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record MembershipAuditMutation(bool Succeeded, string Code, string Message, MembershipAuditRecord? Record, MembershipAuditEvent? Event);
public interface IMembershipAuditRepository
{
    ValueTask<MembershipAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<MembershipAuditPage> QueryAsync(MembershipAuditQuery query, CancellationToken cancellationToken);
    ValueTask<MembershipAuditMutation> SaveAsync(MembershipAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IMembershipAuditEventSink { ValueTask PublishAsync(MembershipAuditEvent domainEvent, CancellationToken cancellationToken); }