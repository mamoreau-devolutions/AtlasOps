namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProtocolPolicyState { Draft, Active, Paused, Completed, Archived }
public sealed record ProtocolPolicyRecord(Guid Id, string Name, string Owner, ProtocolPolicyState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProtocolPolicyCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProtocolPolicyEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProtocolPolicyQuery(string? SearchText, ProtocolPolicyState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProtocolPolicyPage(IReadOnlyList<ProtocolPolicyRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProtocolPolicyMutation(bool Succeeded, string Code, string Message, ProtocolPolicyRecord? Record, ProtocolPolicyEvent? Event);
public interface IProtocolPolicyRepository
{
    ValueTask<ProtocolPolicyRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProtocolPolicyPage> QueryAsync(ProtocolPolicyQuery query, CancellationToken cancellationToken);
    ValueTask<ProtocolPolicyMutation> SaveAsync(ProtocolPolicyRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProtocolPolicyEventSink { ValueTask PublishAsync(ProtocolPolicyEvent domainEvent, CancellationToken cancellationToken); }