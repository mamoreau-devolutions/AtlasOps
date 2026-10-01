namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum VaultAccessState { Draft, Active, Paused, Completed, Archived }
public sealed record VaultAccessRecord(Guid Id, string Name, string Owner, VaultAccessState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record VaultAccessCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record VaultAccessEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record VaultAccessQuery(string? SearchText, VaultAccessState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record VaultAccessPage(IReadOnlyList<VaultAccessRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record VaultAccessMutation(bool Succeeded, string Code, string Message, VaultAccessRecord? Record, VaultAccessEvent? Event);
public interface IVaultAccessRepository
{
    ValueTask<VaultAccessRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<VaultAccessPage> QueryAsync(VaultAccessQuery query, CancellationToken cancellationToken);
    ValueTask<VaultAccessMutation> SaveAsync(VaultAccessRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IVaultAccessEventSink { ValueTask PublishAsync(VaultAccessEvent domainEvent, CancellationToken cancellationToken); }