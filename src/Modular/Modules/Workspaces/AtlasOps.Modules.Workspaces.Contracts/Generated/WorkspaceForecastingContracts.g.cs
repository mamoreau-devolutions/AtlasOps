namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum WorkspaceForecastingState { Draft, Active, Paused, Completed, Archived }
public sealed record WorkspaceForecastingRecord(Guid Id, string Name, string Owner, WorkspaceForecastingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record WorkspaceForecastingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record WorkspaceForecastingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record WorkspaceForecastingQuery(string? SearchText, WorkspaceForecastingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record WorkspaceForecastingPage(IReadOnlyList<WorkspaceForecastingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record WorkspaceForecastingMutation(bool Succeeded, string Code, string Message, WorkspaceForecastingRecord? Record, WorkspaceForecastingEvent? Event);
public interface IWorkspaceForecastingRepository
{
    ValueTask<WorkspaceForecastingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<WorkspaceForecastingPage> QueryAsync(WorkspaceForecastingQuery query, CancellationToken cancellationToken);
    ValueTask<WorkspaceForecastingMutation> SaveAsync(WorkspaceForecastingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IWorkspaceForecastingEventSink { ValueTask PublishAsync(WorkspaceForecastingEvent domainEvent, CancellationToken cancellationToken); }