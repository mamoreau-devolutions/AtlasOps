namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EndpointProvisioningState { Draft, Active, Paused, Completed, Archived }
public sealed record EndpointProvisioningRecord(Guid Id, string Name, string Owner, EndpointProvisioningState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EndpointProvisioningCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EndpointProvisioningEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EndpointProvisioningQuery(string? SearchText, EndpointProvisioningState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EndpointProvisioningPage(IReadOnlyList<EndpointProvisioningRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EndpointProvisioningMutation(bool Succeeded, string Code, string Message, EndpointProvisioningRecord? Record, EndpointProvisioningEvent? Event);
public interface IEndpointProvisioningRepository
{
    ValueTask<EndpointProvisioningRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EndpointProvisioningPage> QueryAsync(EndpointProvisioningQuery query, CancellationToken cancellationToken);
    ValueTask<EndpointProvisioningMutation> SaveAsync(EndpointProvisioningRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEndpointProvisioningEventSink { ValueTask PublishAsync(EndpointProvisioningEvent domainEvent, CancellationToken cancellationToken); }