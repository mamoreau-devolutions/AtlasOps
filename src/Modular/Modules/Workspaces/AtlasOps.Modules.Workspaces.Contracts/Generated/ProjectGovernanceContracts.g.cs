namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProjectGovernanceState { Draft, Active, Paused, Completed, Archived }
public sealed record ProjectGovernanceRecord(Guid Id, string Name, string Owner, ProjectGovernanceState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProjectGovernanceCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProjectGovernanceEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProjectGovernanceQuery(string? SearchText, ProjectGovernanceState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProjectGovernancePage(IReadOnlyList<ProjectGovernanceRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProjectGovernanceMutation(bool Succeeded, string Code, string Message, ProjectGovernanceRecord? Record, ProjectGovernanceEvent? Event);
public interface IProjectGovernanceRepository
{
    ValueTask<ProjectGovernanceRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProjectGovernancePage> QueryAsync(ProjectGovernanceQuery query, CancellationToken cancellationToken);
    ValueTask<ProjectGovernanceMutation> SaveAsync(ProjectGovernanceRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProjectGovernanceEventSink { ValueTask PublishAsync(ProjectGovernanceEvent domainEvent, CancellationToken cancellationToken); }