namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DependencyClassificationState { Draft, Active, Paused, Completed, Archived }
public sealed record DependencyClassificationRecord(Guid Id, string Name, string Owner, DependencyClassificationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DependencyClassificationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DependencyClassificationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DependencyClassificationQuery(string? SearchText, DependencyClassificationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DependencyClassificationPage(IReadOnlyList<DependencyClassificationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DependencyClassificationMutation(bool Succeeded, string Code, string Message, DependencyClassificationRecord? Record, DependencyClassificationEvent? Event);
public interface IDependencyClassificationRepository
{
    ValueTask<DependencyClassificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DependencyClassificationPage> QueryAsync(DependencyClassificationQuery query, CancellationToken cancellationToken);
    ValueTask<DependencyClassificationMutation> SaveAsync(DependencyClassificationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDependencyClassificationEventSink { ValueTask PublishAsync(DependencyClassificationEvent domainEvent, CancellationToken cancellationToken); }