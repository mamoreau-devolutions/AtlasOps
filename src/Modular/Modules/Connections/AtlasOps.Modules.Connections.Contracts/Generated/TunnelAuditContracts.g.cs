namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TunnelAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record TunnelAuditRecord(Guid Id, string Name, string Owner, TunnelAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TunnelAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TunnelAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TunnelAuditQuery(string? SearchText, TunnelAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TunnelAuditPage(IReadOnlyList<TunnelAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TunnelAuditMutation(bool Succeeded, string Code, string Message, TunnelAuditRecord? Record, TunnelAuditEvent? Event);
public interface ITunnelAuditRepository
{
    ValueTask<TunnelAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TunnelAuditPage> QueryAsync(TunnelAuditQuery query, CancellationToken cancellationToken);
    ValueTask<TunnelAuditMutation> SaveAsync(TunnelAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITunnelAuditEventSink { ValueTask PublishAsync(TunnelAuditEvent domainEvent, CancellationToken cancellationToken); }