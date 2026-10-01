namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CredentialRecoveryState { Draft, Active, Paused, Completed, Archived }
public sealed record CredentialRecoveryRecord(Guid Id, string Name, string Owner, CredentialRecoveryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CredentialRecoveryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CredentialRecoveryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CredentialRecoveryQuery(string? SearchText, CredentialRecoveryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CredentialRecoveryPage(IReadOnlyList<CredentialRecoveryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CredentialRecoveryMutation(bool Succeeded, string Code, string Message, CredentialRecoveryRecord? Record, CredentialRecoveryEvent? Event);
public interface ICredentialRecoveryRepository
{
    ValueTask<CredentialRecoveryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CredentialRecoveryPage> QueryAsync(CredentialRecoveryQuery query, CancellationToken cancellationToken);
    ValueTask<CredentialRecoveryMutation> SaveAsync(CredentialRecoveryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICredentialRecoveryEventSink { ValueTask PublishAsync(CredentialRecoveryEvent domainEvent, CancellationToken cancellationToken); }