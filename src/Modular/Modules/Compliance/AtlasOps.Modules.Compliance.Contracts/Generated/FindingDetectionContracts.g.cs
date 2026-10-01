namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum FindingDetectionState { Draft, Active, Paused, Completed, Archived }
public sealed record FindingDetectionRecord(Guid Id, string Name, string Owner, FindingDetectionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record FindingDetectionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record FindingDetectionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record FindingDetectionQuery(string? SearchText, FindingDetectionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record FindingDetectionPage(IReadOnlyList<FindingDetectionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record FindingDetectionMutation(bool Succeeded, string Code, string Message, FindingDetectionRecord? Record, FindingDetectionEvent? Event);
public interface IFindingDetectionRepository
{
    ValueTask<FindingDetectionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<FindingDetectionPage> QueryAsync(FindingDetectionQuery query, CancellationToken cancellationToken);
    ValueTask<FindingDetectionMutation> SaveAsync(FindingDetectionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IFindingDetectionEventSink { ValueTask PublishAsync(FindingDetectionEvent domainEvent, CancellationToken cancellationToken); }