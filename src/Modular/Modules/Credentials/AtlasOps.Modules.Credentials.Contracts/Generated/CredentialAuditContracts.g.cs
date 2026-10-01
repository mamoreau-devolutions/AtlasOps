namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CredentialAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record CredentialAuditRecord(Guid Id, string Name, string Owner, CredentialAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CredentialAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CredentialAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CredentialAuditQuery(string? SearchText, CredentialAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CredentialAuditPage(IReadOnlyList<CredentialAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CredentialAuditMutation(bool Succeeded, string Code, string Message, CredentialAuditRecord? Record, CredentialAuditEvent? Event);
public interface ICredentialAuditRepository
{
    ValueTask<CredentialAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CredentialAuditPage> QueryAsync(CredentialAuditQuery query, CancellationToken cancellationToken);
    ValueTask<CredentialAuditMutation> SaveAsync(CredentialAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICredentialAuditEventSink { ValueTask PublishAsync(CredentialAuditEvent domainEvent, CancellationToken cancellationToken); }