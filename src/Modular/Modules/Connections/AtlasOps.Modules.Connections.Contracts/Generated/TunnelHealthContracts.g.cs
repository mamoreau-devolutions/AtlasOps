namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TunnelHealthState { Draft, Active, Paused, Completed, Archived }
public sealed record TunnelHealthRecord(Guid Id, string Name, string Owner, TunnelHealthState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TunnelHealthCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TunnelHealthEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TunnelHealthQuery(string? SearchText, TunnelHealthState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TunnelHealthPage(IReadOnlyList<TunnelHealthRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TunnelHealthMutation(bool Succeeded, string Code, string Message, TunnelHealthRecord? Record, TunnelHealthEvent? Event);
public interface ITunnelHealthRepository
{
    ValueTask<TunnelHealthRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TunnelHealthPage> QueryAsync(TunnelHealthQuery query, CancellationToken cancellationToken);
    ValueTask<TunnelHealthMutation> SaveAsync(TunnelHealthRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITunnelHealthEventSink { ValueTask PublishAsync(TunnelHealthEvent domainEvent, CancellationToken cancellationToken); }