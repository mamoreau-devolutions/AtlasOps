namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ControlAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record ControlAuditRecord(Guid Id, string Name, string Owner, ControlAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ControlAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ControlAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ControlAuditQuery(string? SearchText, ControlAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ControlAuditPage(IReadOnlyList<ControlAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ControlAuditMutation(bool Succeeded, string Code, string Message, ControlAuditRecord? Record, ControlAuditEvent? Event);
public interface IControlAuditRepository
{
    ValueTask<ControlAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ControlAuditPage> QueryAsync(ControlAuditQuery query, CancellationToken cancellationToken);
    ValueTask<ControlAuditMutation> SaveAsync(ControlAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IControlAuditEventSink { ValueTask PublishAsync(ControlAuditEvent domainEvent, CancellationToken cancellationToken); }