namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum WorkspaceGovernanceState { Draft, Active, Paused, Completed, Archived }
public sealed record WorkspaceGovernanceRecord(Guid Id, string Name, string Owner, WorkspaceGovernanceState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record WorkspaceGovernanceCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record WorkspaceGovernanceEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record WorkspaceGovernanceQuery(string? SearchText, WorkspaceGovernanceState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record WorkspaceGovernancePage(IReadOnlyList<WorkspaceGovernanceRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record WorkspaceGovernanceMutation(bool Succeeded, string Code, string Message, WorkspaceGovernanceRecord? Record, WorkspaceGovernanceEvent? Event);
public interface IWorkspaceGovernanceRepository
{
    ValueTask<WorkspaceGovernanceRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<WorkspaceGovernancePage> QueryAsync(WorkspaceGovernanceQuery query, CancellationToken cancellationToken);
    ValueTask<WorkspaceGovernanceMutation> SaveAsync(WorkspaceGovernanceRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IWorkspaceGovernanceEventSink { ValueTask PublishAsync(WorkspaceGovernanceEvent domainEvent, CancellationToken cancellationToken); }