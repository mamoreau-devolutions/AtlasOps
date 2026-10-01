namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CredentialRotationState { Draft, Active, Paused, Completed, Archived }
public sealed record CredentialRotationRecord(Guid Id, string Name, string Owner, CredentialRotationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CredentialRotationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CredentialRotationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CredentialRotationQuery(string? SearchText, CredentialRotationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CredentialRotationPage(IReadOnlyList<CredentialRotationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CredentialRotationMutation(bool Succeeded, string Code, string Message, CredentialRotationRecord? Record, CredentialRotationEvent? Event);
public interface ICredentialRotationRepository
{
    ValueTask<CredentialRotationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CredentialRotationPage> QueryAsync(CredentialRotationQuery query, CancellationToken cancellationToken);
    ValueTask<CredentialRotationMutation> SaveAsync(CredentialRotationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICredentialRotationEventSink { ValueTask PublishAsync(CredentialRotationEvent domainEvent, CancellationToken cancellationToken); }