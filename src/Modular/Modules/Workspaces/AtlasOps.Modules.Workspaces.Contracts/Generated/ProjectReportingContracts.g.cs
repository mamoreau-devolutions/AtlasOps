namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProjectReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record ProjectReportingRecord(Guid Id, string Name, string Owner, ProjectReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProjectReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProjectReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProjectReportingQuery(string? SearchText, ProjectReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProjectReportingPage(IReadOnlyList<ProjectReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProjectReportingMutation(bool Succeeded, string Code, string Message, ProjectReportingRecord? Record, ProjectReportingEvent? Event);
public interface IProjectReportingRepository
{
    ValueTask<ProjectReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProjectReportingPage> QueryAsync(ProjectReportingQuery query, CancellationToken cancellationToken);
    ValueTask<ProjectReportingMutation> SaveAsync(ProjectReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProjectReportingEventSink { ValueTask PublishAsync(ProjectReportingEvent domainEvent, CancellationToken cancellationToken); }