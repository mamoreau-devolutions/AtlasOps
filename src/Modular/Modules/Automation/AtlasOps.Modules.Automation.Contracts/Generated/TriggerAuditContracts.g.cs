namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TriggerAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record TriggerAuditRecord(Guid Id, string Name, string Owner, TriggerAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TriggerAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TriggerAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TriggerAuditQuery(string? SearchText, TriggerAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TriggerAuditPage(IReadOnlyList<TriggerAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TriggerAuditMutation(bool Succeeded, string Code, string Message, TriggerAuditRecord? Record, TriggerAuditEvent? Event);
public interface ITriggerAuditRepository
{
    ValueTask<TriggerAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TriggerAuditPage> QueryAsync(TriggerAuditQuery query, CancellationToken cancellationToken);
    ValueTask<TriggerAuditMutation> SaveAsync(TriggerAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITriggerAuditEventSink { ValueTask PublishAsync(TriggerAuditEvent domainEvent, CancellationToken cancellationToken); }