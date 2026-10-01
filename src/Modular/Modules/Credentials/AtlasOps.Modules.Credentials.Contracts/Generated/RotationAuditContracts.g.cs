namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RotationAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record RotationAuditRecord(Guid Id, string Name, string Owner, RotationAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RotationAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RotationAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RotationAuditQuery(string? SearchText, RotationAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RotationAuditPage(IReadOnlyList<RotationAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RotationAuditMutation(bool Succeeded, string Code, string Message, RotationAuditRecord? Record, RotationAuditEvent? Event);
public interface IRotationAuditRepository
{
    ValueTask<RotationAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RotationAuditPage> QueryAsync(RotationAuditQuery query, CancellationToken cancellationToken);
    ValueTask<RotationAuditMutation> SaveAsync(RotationAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRotationAuditEventSink { ValueTask PublishAsync(RotationAuditEvent domainEvent, CancellationToken cancellationToken); }