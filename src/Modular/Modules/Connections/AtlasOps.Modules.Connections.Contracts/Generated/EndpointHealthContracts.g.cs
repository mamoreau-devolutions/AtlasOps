namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EndpointHealthState { Draft, Active, Paused, Completed, Archived }
public sealed record EndpointHealthRecord(Guid Id, string Name, string Owner, EndpointHealthState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EndpointHealthCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EndpointHealthEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EndpointHealthQuery(string? SearchText, EndpointHealthState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EndpointHealthPage(IReadOnlyList<EndpointHealthRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EndpointHealthMutation(bool Succeeded, string Code, string Message, EndpointHealthRecord? Record, EndpointHealthEvent? Event);
public interface IEndpointHealthRepository
{
    ValueTask<EndpointHealthRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EndpointHealthPage> QueryAsync(EndpointHealthQuery query, CancellationToken cancellationToken);
    ValueTask<EndpointHealthMutation> SaveAsync(EndpointHealthRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEndpointHealthEventSink { ValueTask PublishAsync(EndpointHealthEvent domainEvent, CancellationToken cancellationToken); }