namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum GatewayProvisioningState { Draft, Active, Paused, Completed, Archived }
public sealed record GatewayProvisioningRecord(Guid Id, string Name, string Owner, GatewayProvisioningState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record GatewayProvisioningCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record GatewayProvisioningEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record GatewayProvisioningQuery(string? SearchText, GatewayProvisioningState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record GatewayProvisioningPage(IReadOnlyList<GatewayProvisioningRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record GatewayProvisioningMutation(bool Succeeded, string Code, string Message, GatewayProvisioningRecord? Record, GatewayProvisioningEvent? Event);
public interface IGatewayProvisioningRepository
{
    ValueTask<GatewayProvisioningRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<GatewayProvisioningPage> QueryAsync(GatewayProvisioningQuery query, CancellationToken cancellationToken);
    ValueTask<GatewayProvisioningMutation> SaveAsync(GatewayProvisioningRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IGatewayProvisioningEventSink { ValueTask PublishAsync(GatewayProvisioningEvent domainEvent, CancellationToken cancellationToken); }