namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum VaultRotationState { Draft, Active, Paused, Completed, Archived }
public sealed record VaultRotationRecord(Guid Id, string Name, string Owner, VaultRotationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record VaultRotationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record VaultRotationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record VaultRotationQuery(string? SearchText, VaultRotationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record VaultRotationPage(IReadOnlyList<VaultRotationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record VaultRotationMutation(bool Succeeded, string Code, string Message, VaultRotationRecord? Record, VaultRotationEvent? Event);
public interface IVaultRotationRepository
{
    ValueTask<VaultRotationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<VaultRotationPage> QueryAsync(VaultRotationQuery query, CancellationToken cancellationToken);
    ValueTask<VaultRotationMutation> SaveAsync(VaultRotationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IVaultRotationEventSink { ValueTask PublishAsync(VaultRotationEvent domainEvent, CancellationToken cancellationToken); }