namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EndpointRoutingState { Draft, Active, Paused, Completed, Archived }
public sealed record EndpointRoutingRecord(Guid Id, string Name, string Owner, EndpointRoutingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EndpointRoutingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EndpointRoutingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EndpointRoutingQuery(string? SearchText, EndpointRoutingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EndpointRoutingPage(IReadOnlyList<EndpointRoutingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EndpointRoutingMutation(bool Succeeded, string Code, string Message, EndpointRoutingRecord? Record, EndpointRoutingEvent? Event);
public interface IEndpointRoutingRepository
{
    ValueTask<EndpointRoutingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EndpointRoutingPage> QueryAsync(EndpointRoutingQuery query, CancellationToken cancellationToken);
    ValueTask<EndpointRoutingMutation> SaveAsync(EndpointRoutingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEndpointRoutingEventSink { ValueTask PublishAsync(EndpointRoutingEvent domainEvent, CancellationToken cancellationToken); }