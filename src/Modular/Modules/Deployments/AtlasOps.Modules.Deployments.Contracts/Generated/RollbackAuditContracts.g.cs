namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RollbackAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record RollbackAuditRecord(Guid Id, string Name, string Owner, RollbackAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RollbackAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RollbackAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RollbackAuditQuery(string? SearchText, RollbackAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RollbackAuditPage(IReadOnlyList<RollbackAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RollbackAuditMutation(bool Succeeded, string Code, string Message, RollbackAuditRecord? Record, RollbackAuditEvent? Event);
public interface IRollbackAuditRepository
{
    ValueTask<RollbackAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RollbackAuditPage> QueryAsync(RollbackAuditQuery query, CancellationToken cancellationToken);
    ValueTask<RollbackAuditMutation> SaveAsync(RollbackAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRollbackAuditEventSink { ValueTask PublishAsync(RollbackAuditEvent domainEvent, CancellationToken cancellationToken); }