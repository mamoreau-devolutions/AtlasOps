namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ComponentDetectionState { Draft, Active, Paused, Completed, Archived }
public sealed record ComponentDetectionRecord(Guid Id, string Name, string Owner, ComponentDetectionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ComponentDetectionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ComponentDetectionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ComponentDetectionQuery(string? SearchText, ComponentDetectionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ComponentDetectionPage(IReadOnlyList<ComponentDetectionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ComponentDetectionMutation(bool Succeeded, string Code, string Message, ComponentDetectionRecord? Record, ComponentDetectionEvent? Event);
public interface IComponentDetectionRepository
{
    ValueTask<ComponentDetectionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ComponentDetectionPage> QueryAsync(ComponentDetectionQuery query, CancellationToken cancellationToken);
    ValueTask<ComponentDetectionMutation> SaveAsync(ComponentDetectionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IComponentDetectionEventSink { ValueTask PublishAsync(ComponentDetectionEvent domainEvent, CancellationToken cancellationToken); }