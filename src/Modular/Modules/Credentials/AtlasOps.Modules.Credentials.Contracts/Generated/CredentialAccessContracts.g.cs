namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CredentialAccessState { Draft, Active, Paused, Completed, Archived }
public sealed record CredentialAccessRecord(Guid Id, string Name, string Owner, CredentialAccessState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CredentialAccessCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CredentialAccessEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CredentialAccessQuery(string? SearchText, CredentialAccessState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CredentialAccessPage(IReadOnlyList<CredentialAccessRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CredentialAccessMutation(bool Succeeded, string Code, string Message, CredentialAccessRecord? Record, CredentialAccessEvent? Event);
public interface ICredentialAccessRepository
{
    ValueTask<CredentialAccessRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CredentialAccessPage> QueryAsync(CredentialAccessQuery query, CancellationToken cancellationToken);
    ValueTask<CredentialAccessMutation> SaveAsync(CredentialAccessRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICredentialAccessEventSink { ValueTask PublishAsync(CredentialAccessEvent domainEvent, CancellationToken cancellationToken); }