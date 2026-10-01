namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum GatewayHealthState { Draft, Active, Paused, Completed, Archived }
public sealed record GatewayHealthRecord(Guid Id, string Name, string Owner, GatewayHealthState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record GatewayHealthCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record GatewayHealthEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record GatewayHealthQuery(string? SearchText, GatewayHealthState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record GatewayHealthPage(IReadOnlyList<GatewayHealthRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record GatewayHealthMutation(bool Succeeded, string Code, string Message, GatewayHealthRecord? Record, GatewayHealthEvent? Event);
public interface IGatewayHealthRepository
{
    ValueTask<GatewayHealthRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<GatewayHealthPage> QueryAsync(GatewayHealthQuery query, CancellationToken cancellationToken);
    ValueTask<GatewayHealthMutation> SaveAsync(GatewayHealthRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IGatewayHealthEventSink { ValueTask PublishAsync(GatewayHealthEvent domainEvent, CancellationToken cancellationToken); }