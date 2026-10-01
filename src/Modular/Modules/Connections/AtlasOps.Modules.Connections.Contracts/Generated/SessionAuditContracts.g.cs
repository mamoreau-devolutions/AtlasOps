namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum SessionAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record SessionAuditRecord(Guid Id, string Name, string Owner, SessionAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record SessionAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SessionAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record SessionAuditQuery(string? SearchText, SessionAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record SessionAuditPage(IReadOnlyList<SessionAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record SessionAuditMutation(bool Succeeded, string Code, string Message, SessionAuditRecord? Record, SessionAuditEvent? Event);
public interface ISessionAuditRepository
{
    ValueTask<SessionAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<SessionAuditPage> QueryAsync(SessionAuditQuery query, CancellationToken cancellationToken);
    ValueTask<SessionAuditMutation> SaveAsync(SessionAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ISessionAuditEventSink { ValueTask PublishAsync(SessionAuditEvent domainEvent, CancellationToken cancellationToken); }