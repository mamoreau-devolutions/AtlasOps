namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum SoftwareClassificationState { Draft, Active, Paused, Completed, Archived }
public sealed record SoftwareClassificationRecord(Guid Id, string Name, string Owner, SoftwareClassificationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record SoftwareClassificationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SoftwareClassificationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record SoftwareClassificationQuery(string? SearchText, SoftwareClassificationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record SoftwareClassificationPage(IReadOnlyList<SoftwareClassificationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record SoftwareClassificationMutation(bool Succeeded, string Code, string Message, SoftwareClassificationRecord? Record, SoftwareClassificationEvent? Event);
public interface ISoftwareClassificationRepository
{
    ValueTask<SoftwareClassificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<SoftwareClassificationPage> QueryAsync(SoftwareClassificationQuery query, CancellationToken cancellationToken);
    ValueTask<SoftwareClassificationMutation> SaveAsync(SoftwareClassificationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ISoftwareClassificationEventSink { ValueTask PublishAsync(SoftwareClassificationEvent domainEvent, CancellationToken cancellationToken); }