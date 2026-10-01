namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PortPolicyState { Draft, Active, Paused, Completed, Archived }
public sealed record PortPolicyRecord(Guid Id, string Name, string Owner, PortPolicyState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PortPolicyCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PortPolicyEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PortPolicyQuery(string? SearchText, PortPolicyState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PortPolicyPage(IReadOnlyList<PortPolicyRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PortPolicyMutation(bool Succeeded, string Code, string Message, PortPolicyRecord? Record, PortPolicyEvent? Event);
public interface IPortPolicyRepository
{
    ValueTask<PortPolicyRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PortPolicyPage> QueryAsync(PortPolicyQuery query, CancellationToken cancellationToken);
    ValueTask<PortPolicyMutation> SaveAsync(PortPolicyRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPortPolicyEventSink { ValueTask PublishAsync(PortPolicyEvent domainEvent, CancellationToken cancellationToken); }