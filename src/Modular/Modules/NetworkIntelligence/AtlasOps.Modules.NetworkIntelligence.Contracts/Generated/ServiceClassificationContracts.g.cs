namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ServiceClassificationState { Draft, Active, Paused, Completed, Archived }
public sealed record ServiceClassificationRecord(Guid Id, string Name, string Owner, ServiceClassificationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ServiceClassificationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ServiceClassificationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ServiceClassificationQuery(string? SearchText, ServiceClassificationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ServiceClassificationPage(IReadOnlyList<ServiceClassificationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ServiceClassificationMutation(bool Succeeded, string Code, string Message, ServiceClassificationRecord? Record, ServiceClassificationEvent? Event);
public interface IServiceClassificationRepository
{
    ValueTask<ServiceClassificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ServiceClassificationPage> QueryAsync(ServiceClassificationQuery query, CancellationToken cancellationToken);
    ValueTask<ServiceClassificationMutation> SaveAsync(ServiceClassificationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IServiceClassificationEventSink { ValueTask PublishAsync(ServiceClassificationEvent domainEvent, CancellationToken cancellationToken); }