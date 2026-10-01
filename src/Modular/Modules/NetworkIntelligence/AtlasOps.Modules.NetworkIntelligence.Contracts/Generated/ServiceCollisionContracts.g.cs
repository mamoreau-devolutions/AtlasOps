namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ServiceCollisionState { Draft, Active, Paused, Completed, Archived }
public sealed record ServiceCollisionRecord(Guid Id, string Name, string Owner, ServiceCollisionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ServiceCollisionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ServiceCollisionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ServiceCollisionQuery(string? SearchText, ServiceCollisionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ServiceCollisionPage(IReadOnlyList<ServiceCollisionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ServiceCollisionMutation(bool Succeeded, string Code, string Message, ServiceCollisionRecord? Record, ServiceCollisionEvent? Event);
public interface IServiceCollisionRepository
{
    ValueTask<ServiceCollisionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ServiceCollisionPage> QueryAsync(ServiceCollisionQuery query, CancellationToken cancellationToken);
    ValueTask<ServiceCollisionMutation> SaveAsync(ServiceCollisionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IServiceCollisionEventSink { ValueTask PublishAsync(ServiceCollisionEvent domainEvent, CancellationToken cancellationToken); }