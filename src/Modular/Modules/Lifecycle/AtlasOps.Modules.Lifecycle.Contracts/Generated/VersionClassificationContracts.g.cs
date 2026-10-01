namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum VersionClassificationState { Draft, Active, Paused, Completed, Archived }
public sealed record VersionClassificationRecord(Guid Id, string Name, string Owner, VersionClassificationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record VersionClassificationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record VersionClassificationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record VersionClassificationQuery(string? SearchText, VersionClassificationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record VersionClassificationPage(IReadOnlyList<VersionClassificationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record VersionClassificationMutation(bool Succeeded, string Code, string Message, VersionClassificationRecord? Record, VersionClassificationEvent? Event);
public interface IVersionClassificationRepository
{
    ValueTask<VersionClassificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<VersionClassificationPage> QueryAsync(VersionClassificationQuery query, CancellationToken cancellationToken);
    ValueTask<VersionClassificationMutation> SaveAsync(VersionClassificationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IVersionClassificationEventSink { ValueTask PublishAsync(VersionClassificationEvent domainEvent, CancellationToken cancellationToken); }