namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum VaultRecoveryState { Draft, Active, Paused, Completed, Archived }
public sealed record VaultRecoveryRecord(Guid Id, string Name, string Owner, VaultRecoveryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record VaultRecoveryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record VaultRecoveryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record VaultRecoveryQuery(string? SearchText, VaultRecoveryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record VaultRecoveryPage(IReadOnlyList<VaultRecoveryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record VaultRecoveryMutation(bool Succeeded, string Code, string Message, VaultRecoveryRecord? Record, VaultRecoveryEvent? Event);
public interface IVaultRecoveryRepository
{
    ValueTask<VaultRecoveryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<VaultRecoveryPage> QueryAsync(VaultRecoveryQuery query, CancellationToken cancellationToken);
    ValueTask<VaultRecoveryMutation> SaveAsync(VaultRecoveryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IVaultRecoveryEventSink { ValueTask PublishAsync(VaultRecoveryEvent domainEvent, CancellationToken cancellationToken); }