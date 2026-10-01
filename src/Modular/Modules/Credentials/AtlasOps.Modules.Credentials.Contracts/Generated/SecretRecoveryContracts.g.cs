namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum SecretRecoveryState { Draft, Active, Paused, Completed, Archived }
public sealed record SecretRecoveryRecord(Guid Id, string Name, string Owner, SecretRecoveryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record SecretRecoveryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SecretRecoveryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record SecretRecoveryQuery(string? SearchText, SecretRecoveryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record SecretRecoveryPage(IReadOnlyList<SecretRecoveryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record SecretRecoveryMutation(bool Succeeded, string Code, string Message, SecretRecoveryRecord? Record, SecretRecoveryEvent? Event);
public interface ISecretRecoveryRepository
{
    ValueTask<SecretRecoveryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<SecretRecoveryPage> QueryAsync(SecretRecoveryQuery query, CancellationToken cancellationToken);
    ValueTask<SecretRecoveryMutation> SaveAsync(SecretRecoveryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ISecretRecoveryEventSink { ValueTask PublishAsync(SecretRecoveryEvent domainEvent, CancellationToken cancellationToken); }