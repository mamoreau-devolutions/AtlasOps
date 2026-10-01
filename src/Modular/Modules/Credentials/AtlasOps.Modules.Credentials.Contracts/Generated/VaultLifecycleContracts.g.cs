namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum VaultLifecycleState { Draft, Active, Paused, Completed, Archived }
public sealed record VaultLifecycleRecord(Guid Id, string Name, string Owner, VaultLifecycleState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record VaultLifecycleCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record VaultLifecycleEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record VaultLifecycleQuery(string? SearchText, VaultLifecycleState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record VaultLifecyclePage(IReadOnlyList<VaultLifecycleRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record VaultLifecycleMutation(bool Succeeded, string Code, string Message, VaultLifecycleRecord? Record, VaultLifecycleEvent? Event);
public interface IVaultLifecycleRepository
{
    ValueTask<VaultLifecycleRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<VaultLifecyclePage> QueryAsync(VaultLifecycleQuery query, CancellationToken cancellationToken);
    ValueTask<VaultLifecycleMutation> SaveAsync(VaultLifecycleRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IVaultLifecycleEventSink { ValueTask PublishAsync(VaultLifecycleEvent domainEvent, CancellationToken cancellationToken); }