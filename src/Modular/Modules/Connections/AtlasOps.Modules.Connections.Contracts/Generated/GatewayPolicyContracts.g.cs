namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum GatewayPolicyState { Draft, Active, Paused, Completed, Archived }
public sealed record GatewayPolicyRecord(Guid Id, string Name, string Owner, GatewayPolicyState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record GatewayPolicyCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record GatewayPolicyEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record GatewayPolicyQuery(string? SearchText, GatewayPolicyState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record GatewayPolicyPage(IReadOnlyList<GatewayPolicyRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record GatewayPolicyMutation(bool Succeeded, string Code, string Message, GatewayPolicyRecord? Record, GatewayPolicyEvent? Event);
public interface IGatewayPolicyRepository
{
    ValueTask<GatewayPolicyRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<GatewayPolicyPage> QueryAsync(GatewayPolicyQuery query, CancellationToken cancellationToken);
    ValueTask<GatewayPolicyMutation> SaveAsync(GatewayPolicyRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IGatewayPolicyEventSink { ValueTask PublishAsync(GatewayPolicyEvent domainEvent, CancellationToken cancellationToken); }