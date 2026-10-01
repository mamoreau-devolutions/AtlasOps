namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ServiceValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record ServiceValidationRecord(Guid Id, string Name, string Owner, ServiceValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ServiceValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ServiceValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ServiceValidationQuery(string? SearchText, ServiceValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ServiceValidationPage(IReadOnlyList<ServiceValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ServiceValidationMutation(bool Succeeded, string Code, string Message, ServiceValidationRecord? Record, ServiceValidationEvent? Event);
public interface IServiceValidationRepository
{
    ValueTask<ServiceValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ServiceValidationPage> QueryAsync(ServiceValidationQuery query, CancellationToken cancellationToken);
    ValueTask<ServiceValidationMutation> SaveAsync(ServiceValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IServiceValidationEventSink { ValueTask PublishAsync(ServiceValidationEvent domainEvent, CancellationToken cancellationToken); }