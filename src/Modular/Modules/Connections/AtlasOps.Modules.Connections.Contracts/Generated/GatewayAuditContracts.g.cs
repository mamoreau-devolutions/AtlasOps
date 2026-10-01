namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum GatewayAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record GatewayAuditRecord(Guid Id, string Name, string Owner, GatewayAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record GatewayAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record GatewayAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record GatewayAuditQuery(string? SearchText, GatewayAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record GatewayAuditPage(IReadOnlyList<GatewayAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record GatewayAuditMutation(bool Succeeded, string Code, string Message, GatewayAuditRecord? Record, GatewayAuditEvent? Event);
public interface IGatewayAuditRepository
{
    ValueTask<GatewayAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<GatewayAuditPage> QueryAsync(GatewayAuditQuery query, CancellationToken cancellationToken);
    ValueTask<GatewayAuditMutation> SaveAsync(GatewayAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IGatewayAuditEventSink { ValueTask PublishAsync(GatewayAuditEvent domainEvent, CancellationToken cancellationToken); }