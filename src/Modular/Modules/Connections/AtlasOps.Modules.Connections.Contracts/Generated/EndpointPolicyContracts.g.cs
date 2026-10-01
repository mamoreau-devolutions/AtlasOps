namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EndpointPolicyState { Draft, Active, Paused, Completed, Archived }
public sealed record EndpointPolicyRecord(Guid Id, string Name, string Owner, EndpointPolicyState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EndpointPolicyCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EndpointPolicyEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EndpointPolicyQuery(string? SearchText, EndpointPolicyState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EndpointPolicyPage(IReadOnlyList<EndpointPolicyRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EndpointPolicyMutation(bool Succeeded, string Code, string Message, EndpointPolicyRecord? Record, EndpointPolicyEvent? Event);
public interface IEndpointPolicyRepository
{
    ValueTask<EndpointPolicyRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EndpointPolicyPage> QueryAsync(EndpointPolicyQuery query, CancellationToken cancellationToken);
    ValueTask<EndpointPolicyMutation> SaveAsync(EndpointPolicyRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEndpointPolicyEventSink { ValueTask PublishAsync(EndpointPolicyEvent domainEvent, CancellationToken cancellationToken); }