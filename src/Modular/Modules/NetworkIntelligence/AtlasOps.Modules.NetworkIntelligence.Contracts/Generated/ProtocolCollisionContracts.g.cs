namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProtocolCollisionState { Draft, Active, Paused, Completed, Archived }
public sealed record ProtocolCollisionRecord(Guid Id, string Name, string Owner, ProtocolCollisionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProtocolCollisionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProtocolCollisionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProtocolCollisionQuery(string? SearchText, ProtocolCollisionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProtocolCollisionPage(IReadOnlyList<ProtocolCollisionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProtocolCollisionMutation(bool Succeeded, string Code, string Message, ProtocolCollisionRecord? Record, ProtocolCollisionEvent? Event);
public interface IProtocolCollisionRepository
{
    ValueTask<ProtocolCollisionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProtocolCollisionPage> QueryAsync(ProtocolCollisionQuery query, CancellationToken cancellationToken);
    ValueTask<ProtocolCollisionMutation> SaveAsync(ProtocolCollisionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProtocolCollisionEventSink { ValueTask PublishAsync(ProtocolCollisionEvent domainEvent, CancellationToken cancellationToken); }