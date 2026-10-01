namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ReleasePlanningState { Draft, Active, Paused, Completed, Archived }
public sealed record ReleasePlanningRecord(Guid Id, string Name, string Owner, ReleasePlanningState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ReleasePlanningCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ReleasePlanningEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ReleasePlanningQuery(string? SearchText, ReleasePlanningState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ReleasePlanningPage(IReadOnlyList<ReleasePlanningRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ReleasePlanningMutation(bool Succeeded, string Code, string Message, ReleasePlanningRecord? Record, ReleasePlanningEvent? Event);
public interface IReleasePlanningRepository
{
    ValueTask<ReleasePlanningRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ReleasePlanningPage> QueryAsync(ReleasePlanningQuery query, CancellationToken cancellationToken);
    ValueTask<ReleasePlanningMutation> SaveAsync(ReleasePlanningRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IReleasePlanningEventSink { ValueTask PublishAsync(ReleasePlanningEvent domainEvent, CancellationToken cancellationToken); }