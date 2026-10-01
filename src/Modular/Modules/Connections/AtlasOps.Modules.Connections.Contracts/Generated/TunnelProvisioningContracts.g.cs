namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TunnelProvisioningState { Draft, Active, Paused, Completed, Archived }
public sealed record TunnelProvisioningRecord(Guid Id, string Name, string Owner, TunnelProvisioningState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TunnelProvisioningCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TunnelProvisioningEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TunnelProvisioningQuery(string? SearchText, TunnelProvisioningState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TunnelProvisioningPage(IReadOnlyList<TunnelProvisioningRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TunnelProvisioningMutation(bool Succeeded, string Code, string Message, TunnelProvisioningRecord? Record, TunnelProvisioningEvent? Event);
public interface ITunnelProvisioningRepository
{
    ValueTask<TunnelProvisioningRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TunnelProvisioningPage> QueryAsync(TunnelProvisioningQuery query, CancellationToken cancellationToken);
    ValueTask<TunnelProvisioningMutation> SaveAsync(TunnelProvisioningRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITunnelProvisioningEventSink { ValueTask PublishAsync(TunnelProvisioningEvent domainEvent, CancellationToken cancellationToken); }