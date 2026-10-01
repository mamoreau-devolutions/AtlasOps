namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EndpointRecoveryState { Draft, Active, Paused, Completed, Archived }
public sealed record EndpointRecoveryRecord(Guid Id, string Name, string Owner, EndpointRecoveryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EndpointRecoveryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EndpointRecoveryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EndpointRecoveryQuery(string? SearchText, EndpointRecoveryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EndpointRecoveryPage(IReadOnlyList<EndpointRecoveryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EndpointRecoveryMutation(bool Succeeded, string Code, string Message, EndpointRecoveryRecord? Record, EndpointRecoveryEvent? Event);
public interface IEndpointRecoveryRepository
{
    ValueTask<EndpointRecoveryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EndpointRecoveryPage> QueryAsync(EndpointRecoveryQuery query, CancellationToken cancellationToken);
    ValueTask<EndpointRecoveryMutation> SaveAsync(EndpointRecoveryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEndpointRecoveryEventSink { ValueTask PublishAsync(EndpointRecoveryEvent domainEvent, CancellationToken cancellationToken); }