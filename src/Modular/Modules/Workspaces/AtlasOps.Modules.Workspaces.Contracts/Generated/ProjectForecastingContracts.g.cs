namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProjectForecastingState { Draft, Active, Paused, Completed, Archived }
public sealed record ProjectForecastingRecord(Guid Id, string Name, string Owner, ProjectForecastingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProjectForecastingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProjectForecastingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProjectForecastingQuery(string? SearchText, ProjectForecastingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProjectForecastingPage(IReadOnlyList<ProjectForecastingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProjectForecastingMutation(bool Succeeded, string Code, string Message, ProjectForecastingRecord? Record, ProjectForecastingEvent? Event);
public interface IProjectForecastingRepository
{
    ValueTask<ProjectForecastingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProjectForecastingPage> QueryAsync(ProjectForecastingQuery query, CancellationToken cancellationToken);
    ValueTask<ProjectForecastingMutation> SaveAsync(ProjectForecastingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProjectForecastingEventSink { ValueTask PublishAsync(ProjectForecastingEvent domainEvent, CancellationToken cancellationToken); }