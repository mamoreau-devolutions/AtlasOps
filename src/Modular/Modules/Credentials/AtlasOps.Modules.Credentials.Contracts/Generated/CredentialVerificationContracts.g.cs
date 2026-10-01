namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CredentialVerificationState { Draft, Active, Paused, Completed, Archived }
public sealed record CredentialVerificationRecord(Guid Id, string Name, string Owner, CredentialVerificationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CredentialVerificationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CredentialVerificationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CredentialVerificationQuery(string? SearchText, CredentialVerificationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CredentialVerificationPage(IReadOnlyList<CredentialVerificationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CredentialVerificationMutation(bool Succeeded, string Code, string Message, CredentialVerificationRecord? Record, CredentialVerificationEvent? Event);
public interface ICredentialVerificationRepository
{
    ValueTask<CredentialVerificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CredentialVerificationPage> QueryAsync(CredentialVerificationQuery query, CancellationToken cancellationToken);
    ValueTask<CredentialVerificationMutation> SaveAsync(CredentialVerificationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICredentialVerificationEventSink { ValueTask PublishAsync(CredentialVerificationEvent domainEvent, CancellationToken cancellationToken); }