namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PortCollisionState { Draft, Active, Paused, Completed, Archived }
public sealed record PortCollisionRecord(Guid Id, string Name, string Owner, PortCollisionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PortCollisionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PortCollisionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PortCollisionQuery(string? SearchText, PortCollisionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PortCollisionPage(IReadOnlyList<PortCollisionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PortCollisionMutation(bool Succeeded, string Code, string Message, PortCollisionRecord? Record, PortCollisionEvent? Event);
public interface IPortCollisionRepository
{
    ValueTask<PortCollisionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PortCollisionPage> QueryAsync(PortCollisionQuery query, CancellationToken cancellationToken);
    ValueTask<PortCollisionMutation> SaveAsync(PortCollisionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPortCollisionEventSink { ValueTask PublishAsync(PortCollisionEvent domainEvent, CancellationToken cancellationToken); }