namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CycleClassificationState { Draft, Active, Paused, Completed, Archived }
public sealed record CycleClassificationRecord(Guid Id, string Name, string Owner, CycleClassificationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CycleClassificationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CycleClassificationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CycleClassificationQuery(string? SearchText, CycleClassificationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CycleClassificationPage(IReadOnlyList<CycleClassificationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CycleClassificationMutation(bool Succeeded, string Code, string Message, CycleClassificationRecord? Record, CycleClassificationEvent? Event);
public interface ICycleClassificationRepository
{
    ValueTask<CycleClassificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CycleClassificationPage> QueryAsync(CycleClassificationQuery query, CancellationToken cancellationToken);
    ValueTask<CycleClassificationMutation> SaveAsync(CycleClassificationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICycleClassificationEventSink { ValueTask PublishAsync(CycleClassificationEvent domainEvent, CancellationToken cancellationToken); }