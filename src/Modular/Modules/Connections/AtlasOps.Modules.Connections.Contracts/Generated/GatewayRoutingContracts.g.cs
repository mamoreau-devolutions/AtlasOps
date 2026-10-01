namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum GatewayRoutingState { Draft, Active, Paused, Completed, Archived }
public sealed record GatewayRoutingRecord(Guid Id, string Name, string Owner, GatewayRoutingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record GatewayRoutingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record GatewayRoutingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record GatewayRoutingQuery(string? SearchText, GatewayRoutingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record GatewayRoutingPage(IReadOnlyList<GatewayRoutingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record GatewayRoutingMutation(bool Succeeded, string Code, string Message, GatewayRoutingRecord? Record, GatewayRoutingEvent? Event);
public interface IGatewayRoutingRepository
{
    ValueTask<GatewayRoutingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<GatewayRoutingPage> QueryAsync(GatewayRoutingQuery query, CancellationToken cancellationToken);
    ValueTask<GatewayRoutingMutation> SaveAsync(GatewayRoutingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IGatewayRoutingEventSink { ValueTask PublishAsync(GatewayRoutingEvent domainEvent, CancellationToken cancellationToken); }