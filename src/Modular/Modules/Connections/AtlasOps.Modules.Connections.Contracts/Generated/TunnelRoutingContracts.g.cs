namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TunnelRoutingState { Draft, Active, Paused, Completed, Archived }
public sealed record TunnelRoutingRecord(Guid Id, string Name, string Owner, TunnelRoutingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TunnelRoutingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TunnelRoutingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TunnelRoutingQuery(string? SearchText, TunnelRoutingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TunnelRoutingPage(IReadOnlyList<TunnelRoutingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TunnelRoutingMutation(bool Succeeded, string Code, string Message, TunnelRoutingRecord? Record, TunnelRoutingEvent? Event);
public interface ITunnelRoutingRepository
{
    ValueTask<TunnelRoutingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TunnelRoutingPage> QueryAsync(TunnelRoutingQuery query, CancellationToken cancellationToken);
    ValueTask<TunnelRoutingMutation> SaveAsync(TunnelRoutingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITunnelRoutingEventSink { ValueTask PublishAsync(TunnelRoutingEvent domainEvent, CancellationToken cancellationToken); }