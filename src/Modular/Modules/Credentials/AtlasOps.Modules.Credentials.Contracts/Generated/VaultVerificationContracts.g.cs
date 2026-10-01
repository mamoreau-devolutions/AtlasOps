namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum VaultVerificationState { Draft, Active, Paused, Completed, Archived }
public sealed record VaultVerificationRecord(Guid Id, string Name, string Owner, VaultVerificationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record VaultVerificationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record VaultVerificationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record VaultVerificationQuery(string? SearchText, VaultVerificationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record VaultVerificationPage(IReadOnlyList<VaultVerificationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record VaultVerificationMutation(bool Succeeded, string Code, string Message, VaultVerificationRecord? Record, VaultVerificationEvent? Event);
public interface IVaultVerificationRepository
{
    ValueTask<VaultVerificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<VaultVerificationPage> QueryAsync(VaultVerificationQuery query, CancellationToken cancellationToken);
    ValueTask<VaultVerificationMutation> SaveAsync(VaultVerificationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IVaultVerificationEventSink { ValueTask PublishAsync(VaultVerificationEvent domainEvent, CancellationToken cancellationToken); }