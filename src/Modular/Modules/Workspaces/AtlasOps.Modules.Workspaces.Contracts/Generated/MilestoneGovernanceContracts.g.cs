namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum MilestoneGovernanceState { Draft, Active, Paused, Completed, Archived }
public sealed record MilestoneGovernanceRecord(Guid Id, string Name, string Owner, MilestoneGovernanceState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record MilestoneGovernanceCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record MilestoneGovernanceEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record MilestoneGovernanceQuery(string? SearchText, MilestoneGovernanceState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record MilestoneGovernancePage(IReadOnlyList<MilestoneGovernanceRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record MilestoneGovernanceMutation(bool Succeeded, string Code, string Message, MilestoneGovernanceRecord? Record, MilestoneGovernanceEvent? Event);
public interface IMilestoneGovernanceRepository
{
    ValueTask<MilestoneGovernanceRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<MilestoneGovernancePage> QueryAsync(MilestoneGovernanceQuery query, CancellationToken cancellationToken);
    ValueTask<MilestoneGovernanceMutation> SaveAsync(MilestoneGovernanceRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IMilestoneGovernanceEventSink { ValueTask PublishAsync(MilestoneGovernanceEvent domainEvent, CancellationToken cancellationToken); }