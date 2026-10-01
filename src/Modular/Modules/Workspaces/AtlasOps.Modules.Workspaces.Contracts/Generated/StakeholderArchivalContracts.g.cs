namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum StakeholderArchivalState { Draft, Active, Paused, Completed, Archived }
public sealed record StakeholderArchivalRecord(Guid Id, string Name, string Owner, StakeholderArchivalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record StakeholderArchivalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record StakeholderArchivalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record StakeholderArchivalQuery(string? SearchText, StakeholderArchivalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record StakeholderArchivalPage(IReadOnlyList<StakeholderArchivalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record StakeholderArchivalMutation(bool Succeeded, string Code, string Message, StakeholderArchivalRecord? Record, StakeholderArchivalEvent? Event);
public interface IStakeholderArchivalRepository
{
    ValueTask<StakeholderArchivalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<StakeholderArchivalPage> QueryAsync(StakeholderArchivalQuery query, CancellationToken cancellationToken);
    ValueTask<StakeholderArchivalMutation> SaveAsync(StakeholderArchivalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IStakeholderArchivalEventSink { ValueTask PublishAsync(StakeholderArchivalEvent domainEvent, CancellationToken cancellationToken); }