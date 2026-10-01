namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum SecretVerificationState { Draft, Active, Paused, Completed, Archived }
public sealed record SecretVerificationRecord(Guid Id, string Name, string Owner, SecretVerificationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record SecretVerificationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SecretVerificationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record SecretVerificationQuery(string? SearchText, SecretVerificationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record SecretVerificationPage(IReadOnlyList<SecretVerificationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record SecretVerificationMutation(bool Succeeded, string Code, string Message, SecretVerificationRecord? Record, SecretVerificationEvent? Event);
public interface ISecretVerificationRepository
{
    ValueTask<SecretVerificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<SecretVerificationPage> QueryAsync(SecretVerificationQuery query, CancellationToken cancellationToken);
    ValueTask<SecretVerificationMutation> SaveAsync(SecretVerificationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ISecretVerificationEventSink { ValueTask PublishAsync(SecretVerificationEvent domainEvent, CancellationToken cancellationToken); }