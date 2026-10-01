namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum WorkspaceReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record WorkspaceReportingRecord(Guid Id, string Name, string Owner, WorkspaceReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record WorkspaceReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record WorkspaceReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record WorkspaceReportingQuery(string? SearchText, WorkspaceReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record WorkspaceReportingPage(IReadOnlyList<WorkspaceReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record WorkspaceReportingMutation(bool Succeeded, string Code, string Message, WorkspaceReportingRecord? Record, WorkspaceReportingEvent? Event);
public interface IWorkspaceReportingRepository
{
    ValueTask<WorkspaceReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<WorkspaceReportingPage> QueryAsync(WorkspaceReportingQuery query, CancellationToken cancellationToken);
    ValueTask<WorkspaceReportingMutation> SaveAsync(WorkspaceReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IWorkspaceReportingEventSink { ValueTask PublishAsync(WorkspaceReportingEvent domainEvent, CancellationToken cancellationToken); }