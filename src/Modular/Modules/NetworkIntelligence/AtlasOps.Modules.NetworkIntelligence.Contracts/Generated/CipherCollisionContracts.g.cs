namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CipherCollisionState { Draft, Active, Paused, Completed, Archived }
public sealed record CipherCollisionRecord(Guid Id, string Name, string Owner, CipherCollisionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CipherCollisionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CipherCollisionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CipherCollisionQuery(string? SearchText, CipherCollisionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CipherCollisionPage(IReadOnlyList<CipherCollisionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CipherCollisionMutation(bool Succeeded, string Code, string Message, CipherCollisionRecord? Record, CipherCollisionEvent? Event);
public interface ICipherCollisionRepository
{
    ValueTask<CipherCollisionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CipherCollisionPage> QueryAsync(CipherCollisionQuery query, CancellationToken cancellationToken);
    ValueTask<CipherCollisionMutation> SaveAsync(CipherCollisionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICipherCollisionEventSink { ValueTask PublishAsync(CipherCollisionEvent domainEvent, CancellationToken cancellationToken); }