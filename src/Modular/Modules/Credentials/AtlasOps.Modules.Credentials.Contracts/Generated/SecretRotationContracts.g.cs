namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum SecretRotationState { Draft, Active, Paused, Completed, Archived }
public sealed record SecretRotationRecord(Guid Id, string Name, string Owner, SecretRotationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record SecretRotationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SecretRotationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record SecretRotationQuery(string? SearchText, SecretRotationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record SecretRotationPage(IReadOnlyList<SecretRotationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record SecretRotationMutation(bool Succeeded, string Code, string Message, SecretRotationRecord? Record, SecretRotationEvent? Event);
public interface ISecretRotationRepository
{
    ValueTask<SecretRotationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<SecretRotationPage> QueryAsync(SecretRotationQuery query, CancellationToken cancellationToken);
    ValueTask<SecretRotationMutation> SaveAsync(SecretRotationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ISecretRotationEventSink { ValueTask PublishAsync(SecretRotationEvent domainEvent, CancellationToken cancellationToken); }