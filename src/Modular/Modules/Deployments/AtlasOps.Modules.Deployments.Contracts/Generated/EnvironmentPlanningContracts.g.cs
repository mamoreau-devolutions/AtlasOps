namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EnvironmentPlanningState { Draft, Active, Paused, Completed, Archived }
public sealed record EnvironmentPlanningRecord(Guid Id, string Name, string Owner, EnvironmentPlanningState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EnvironmentPlanningCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EnvironmentPlanningEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EnvironmentPlanningQuery(string? SearchText, EnvironmentPlanningState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EnvironmentPlanningPage(IReadOnlyList<EnvironmentPlanningRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EnvironmentPlanningMutation(bool Succeeded, string Code, string Message, EnvironmentPlanningRecord? Record, EnvironmentPlanningEvent? Event);
public interface IEnvironmentPlanningRepository
{
    ValueTask<EnvironmentPlanningRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EnvironmentPlanningPage> QueryAsync(EnvironmentPlanningQuery query, CancellationToken cancellationToken);
    ValueTask<EnvironmentPlanningMutation> SaveAsync(EnvironmentPlanningRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEnvironmentPlanningEventSink { ValueTask PublishAsync(EnvironmentPlanningEvent domainEvent, CancellationToken cancellationToken); }