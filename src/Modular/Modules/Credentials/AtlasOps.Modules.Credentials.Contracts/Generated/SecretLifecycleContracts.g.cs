namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum SecretLifecycleState { Draft, Active, Paused, Completed, Archived }
public sealed record SecretLifecycleRecord(Guid Id, string Name, string Owner, SecretLifecycleState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record SecretLifecycleCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SecretLifecycleEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record SecretLifecycleQuery(string? SearchText, SecretLifecycleState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record SecretLifecyclePage(IReadOnlyList<SecretLifecycleRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record SecretLifecycleMutation(bool Succeeded, string Code, string Message, SecretLifecycleRecord? Record, SecretLifecycleEvent? Event);
public interface ISecretLifecycleRepository
{
    ValueTask<SecretLifecycleRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<SecretLifecyclePage> QueryAsync(SecretLifecycleQuery query, CancellationToken cancellationToken);
    ValueTask<SecretLifecycleMutation> SaveAsync(SecretLifecycleRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ISecretLifecycleEventSink { ValueTask PublishAsync(SecretLifecycleEvent domainEvent, CancellationToken cancellationToken); }