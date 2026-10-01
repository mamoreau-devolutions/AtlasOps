namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum SecretAccessState { Draft, Active, Paused, Completed, Archived }
public sealed record SecretAccessRecord(Guid Id, string Name, string Owner, SecretAccessState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record SecretAccessCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SecretAccessEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record SecretAccessQuery(string? SearchText, SecretAccessState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record SecretAccessPage(IReadOnlyList<SecretAccessRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record SecretAccessMutation(bool Succeeded, string Code, string Message, SecretAccessRecord? Record, SecretAccessEvent? Event);
public interface ISecretAccessRepository
{
    ValueTask<SecretAccessRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<SecretAccessPage> QueryAsync(SecretAccessQuery query, CancellationToken cancellationToken);
    ValueTask<SecretAccessMutation> SaveAsync(SecretAccessRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ISecretAccessEventSink { ValueTask PublishAsync(SecretAccessEvent domainEvent, CancellationToken cancellationToken); }