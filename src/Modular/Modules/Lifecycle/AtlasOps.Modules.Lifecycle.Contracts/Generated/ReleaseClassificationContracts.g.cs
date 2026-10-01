namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ReleaseClassificationState { Draft, Active, Paused, Completed, Archived }
public sealed record ReleaseClassificationRecord(Guid Id, string Name, string Owner, ReleaseClassificationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ReleaseClassificationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ReleaseClassificationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ReleaseClassificationQuery(string? SearchText, ReleaseClassificationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ReleaseClassificationPage(IReadOnlyList<ReleaseClassificationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ReleaseClassificationMutation(bool Succeeded, string Code, string Message, ReleaseClassificationRecord? Record, ReleaseClassificationEvent? Event);
public interface IReleaseClassificationRepository
{
    ValueTask<ReleaseClassificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ReleaseClassificationPage> QueryAsync(ReleaseClassificationQuery query, CancellationToken cancellationToken);
    ValueTask<ReleaseClassificationMutation> SaveAsync(ReleaseClassificationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IReleaseClassificationEventSink { ValueTask PublishAsync(ReleaseClassificationEvent domainEvent, CancellationToken cancellationToken); }