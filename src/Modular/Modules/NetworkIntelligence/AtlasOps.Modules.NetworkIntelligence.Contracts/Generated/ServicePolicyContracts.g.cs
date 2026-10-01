namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ServicePolicyState { Draft, Active, Paused, Completed, Archived }
public sealed record ServicePolicyRecord(Guid Id, string Name, string Owner, ServicePolicyState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ServicePolicyCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ServicePolicyEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ServicePolicyQuery(string? SearchText, ServicePolicyState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ServicePolicyPage(IReadOnlyList<ServicePolicyRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ServicePolicyMutation(bool Succeeded, string Code, string Message, ServicePolicyRecord? Record, ServicePolicyEvent? Event);
public interface IServicePolicyRepository
{
    ValueTask<ServicePolicyRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ServicePolicyPage> QueryAsync(ServicePolicyQuery query, CancellationToken cancellationToken);
    ValueTask<ServicePolicyMutation> SaveAsync(ServicePolicyRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IServicePolicyEventSink { ValueTask PublishAsync(ServicePolicyEvent domainEvent, CancellationToken cancellationToken); }