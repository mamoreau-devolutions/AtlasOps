namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TunnelRecoveryState { Draft, Active, Paused, Completed, Archived }
public sealed record TunnelRecoveryRecord(Guid Id, string Name, string Owner, TunnelRecoveryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TunnelRecoveryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TunnelRecoveryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TunnelRecoveryQuery(string? SearchText, TunnelRecoveryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TunnelRecoveryPage(IReadOnlyList<TunnelRecoveryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TunnelRecoveryMutation(bool Succeeded, string Code, string Message, TunnelRecoveryRecord? Record, TunnelRecoveryEvent? Event);
public interface ITunnelRecoveryRepository
{
    ValueTask<TunnelRecoveryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TunnelRecoveryPage> QueryAsync(TunnelRecoveryQuery query, CancellationToken cancellationToken);
    ValueTask<TunnelRecoveryMutation> SaveAsync(TunnelRecoveryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITunnelRecoveryEventSink { ValueTask PublishAsync(TunnelRecoveryEvent domainEvent, CancellationToken cancellationToken); }