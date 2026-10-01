namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TunnelPolicyState { Draft, Active, Paused, Completed, Archived }
public sealed record TunnelPolicyRecord(Guid Id, string Name, string Owner, TunnelPolicyState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TunnelPolicyCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TunnelPolicyEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TunnelPolicyQuery(string? SearchText, TunnelPolicyState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TunnelPolicyPage(IReadOnlyList<TunnelPolicyRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TunnelPolicyMutation(bool Succeeded, string Code, string Message, TunnelPolicyRecord? Record, TunnelPolicyEvent? Event);
public interface ITunnelPolicyRepository
{
    ValueTask<TunnelPolicyRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TunnelPolicyPage> QueryAsync(TunnelPolicyQuery query, CancellationToken cancellationToken);
    ValueTask<TunnelPolicyMutation> SaveAsync(TunnelPolicyRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITunnelPolicyEventSink { ValueTask PublishAsync(TunnelPolicyEvent domainEvent, CancellationToken cancellationToken); }