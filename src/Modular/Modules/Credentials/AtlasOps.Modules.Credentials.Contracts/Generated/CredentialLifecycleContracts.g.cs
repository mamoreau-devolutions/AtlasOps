namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CredentialLifecycleState { Draft, Active, Paused, Completed, Archived }
public sealed record CredentialLifecycleRecord(Guid Id, string Name, string Owner, CredentialLifecycleState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CredentialLifecycleCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CredentialLifecycleEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CredentialLifecycleQuery(string? SearchText, CredentialLifecycleState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CredentialLifecyclePage(IReadOnlyList<CredentialLifecycleRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CredentialLifecycleMutation(bool Succeeded, string Code, string Message, CredentialLifecycleRecord? Record, CredentialLifecycleEvent? Event);
public interface ICredentialLifecycleRepository
{
    ValueTask<CredentialLifecycleRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CredentialLifecyclePage> QueryAsync(CredentialLifecycleQuery query, CancellationToken cancellationToken);
    ValueTask<CredentialLifecycleMutation> SaveAsync(CredentialLifecycleRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICredentialLifecycleEventSink { ValueTask PublishAsync(CredentialLifecycleEvent domainEvent, CancellationToken cancellationToken); }