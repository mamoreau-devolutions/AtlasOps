namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum StakeholderGovernanceState { Draft, Active, Paused, Completed, Archived }
public sealed record StakeholderGovernanceRecord(Guid Id, string Name, string Owner, StakeholderGovernanceState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record StakeholderGovernanceCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record StakeholderGovernanceEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record StakeholderGovernanceQuery(string? SearchText, StakeholderGovernanceState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record StakeholderGovernancePage(IReadOnlyList<StakeholderGovernanceRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record StakeholderGovernanceMutation(bool Succeeded, string Code, string Message, StakeholderGovernanceRecord? Record, StakeholderGovernanceEvent? Event);
public interface IStakeholderGovernanceRepository
{
    ValueTask<StakeholderGovernanceRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<StakeholderGovernancePage> QueryAsync(StakeholderGovernanceQuery query, CancellationToken cancellationToken);
    ValueTask<StakeholderGovernanceMutation> SaveAsync(StakeholderGovernanceRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IStakeholderGovernanceEventSink { ValueTask PublishAsync(StakeholderGovernanceEvent domainEvent, CancellationToken cancellationToken); }