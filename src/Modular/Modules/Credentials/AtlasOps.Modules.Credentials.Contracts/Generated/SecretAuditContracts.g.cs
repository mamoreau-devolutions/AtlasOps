namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum SecretAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record SecretAuditRecord(Guid Id, string Name, string Owner, SecretAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record SecretAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SecretAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record SecretAuditQuery(string? SearchText, SecretAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record SecretAuditPage(IReadOnlyList<SecretAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record SecretAuditMutation(bool Succeeded, string Code, string Message, SecretAuditRecord? Record, SecretAuditEvent? Event);
public interface ISecretAuditRepository
{
    ValueTask<SecretAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<SecretAuditPage> QueryAsync(SecretAuditQuery query, CancellationToken cancellationToken);
    ValueTask<SecretAuditMutation> SaveAsync(SecretAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ISecretAuditEventSink { ValueTask PublishAsync(SecretAuditEvent domainEvent, CancellationToken cancellationToken); }