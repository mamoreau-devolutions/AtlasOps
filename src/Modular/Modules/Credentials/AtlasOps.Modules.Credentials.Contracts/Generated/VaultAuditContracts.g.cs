namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum VaultAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record VaultAuditRecord(Guid Id, string Name, string Owner, VaultAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record VaultAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record VaultAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record VaultAuditQuery(string? SearchText, VaultAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record VaultAuditPage(IReadOnlyList<VaultAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record VaultAuditMutation(bool Succeeded, string Code, string Message, VaultAuditRecord? Record, VaultAuditEvent? Event);
public interface IVaultAuditRepository
{
    ValueTask<VaultAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<VaultAuditPage> QueryAsync(VaultAuditQuery query, CancellationToken cancellationToken);
    ValueTask<VaultAuditMutation> SaveAsync(VaultAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IVaultAuditEventSink { ValueTask PublishAsync(VaultAuditEvent domainEvent, CancellationToken cancellationToken); }