namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum WorkspaceArchivalState { Draft, Active, Paused, Completed, Archived }
public sealed record WorkspaceArchivalRecord(Guid Id, string Name, string Owner, WorkspaceArchivalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record WorkspaceArchivalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record WorkspaceArchivalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record WorkspaceArchivalQuery(string? SearchText, WorkspaceArchivalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record WorkspaceArchivalPage(IReadOnlyList<WorkspaceArchivalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record WorkspaceArchivalMutation(bool Succeeded, string Code, string Message, WorkspaceArchivalRecord? Record, WorkspaceArchivalEvent? Event);
public interface IWorkspaceArchivalRepository
{
    ValueTask<WorkspaceArchivalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<WorkspaceArchivalPage> QueryAsync(WorkspaceArchivalQuery query, CancellationToken cancellationToken);
    ValueTask<WorkspaceArchivalMutation> SaveAsync(WorkspaceArchivalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IWorkspaceArchivalEventSink { ValueTask PublishAsync(WorkspaceArchivalEvent domainEvent, CancellationToken cancellationToken); }