namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum GatewayRecoveryState { Draft, Active, Paused, Completed, Archived }
public sealed record GatewayRecoveryRecord(Guid Id, string Name, string Owner, GatewayRecoveryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record GatewayRecoveryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record GatewayRecoveryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record GatewayRecoveryQuery(string? SearchText, GatewayRecoveryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record GatewayRecoveryPage(IReadOnlyList<GatewayRecoveryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record GatewayRecoveryMutation(bool Succeeded, string Code, string Message, GatewayRecoveryRecord? Record, GatewayRecoveryEvent? Event);
public interface IGatewayRecoveryRepository
{
    ValueTask<GatewayRecoveryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<GatewayRecoveryPage> QueryAsync(GatewayRecoveryQuery query, CancellationToken cancellationToken);
    ValueTask<GatewayRecoveryMutation> SaveAsync(GatewayRecoveryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IGatewayRecoveryEventSink { ValueTask PublishAsync(GatewayRecoveryEvent domainEvent, CancellationToken cancellationToken); }